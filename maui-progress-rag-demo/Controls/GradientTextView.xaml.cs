using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Maui.Devices;

namespace MauiProgressRagDemo.Controls;

/// <summary>
/// Simplified gradient text view that always applies one gradient over the full text block.
/// </summary>
public partial class GradientTextView : ContentView
{
    #region Fields

    private INotifyCollectionChanged? _attachedLinesCollection;
    private float _lastCanvasScale;

    #endregion

    #region Bindable Properties

    public static readonly BindableProperty LinesProperty =
        BindableProperty.Create(nameof(Lines), typeof(IList<string>), typeof(GradientTextView), defaultValue: null,
            propertyChanged: OnVisualPropertyChanged,
            defaultValueCreator: _ => new ObservableCollection<string>());

    public static readonly BindableProperty GradientProperty =
        BindableProperty.Create(nameof(Gradient), typeof(LinearGradientBrush), typeof(GradientTextView),
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(GradientTextView), 60d,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty HorizontalTextAlignmentProperty =
        BindableProperty.Create(nameof(HorizontalTextAlignment), typeof(TextAlignment), typeof(GradientTextView), TextAlignment.Center,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty LineHeightProperty =
        BindableProperty.Create(nameof(LineHeight), typeof(double), typeof(GradientTextView), 0.9d,
            propertyChanged: OnVisualPropertyChanged);

    public static readonly BindableProperty LetterSpacingProperty =
        BindableProperty.Create(nameof(LetterSpacing), typeof(double), typeof(GradientTextView), 0d,
            propertyChanged: OnVisualPropertyChanged);

    #endregion

    #region Properties

    public IList<string> Lines
    {
        get => (IList<string>)this.GetValue(LinesProperty);
        set => this.SetValue(LinesProperty, value);
    }

    public LinearGradientBrush? Gradient
    {
        get => (LinearGradientBrush?)this.GetValue(GradientProperty);
        set => this.SetValue(GradientProperty, value);
    }

    public double FontSize
    {
        get => (double)this.GetValue(FontSizeProperty);
        set => this.SetValue(FontSizeProperty, value);
    }

    public TextAlignment HorizontalTextAlignment
    {
        get => (TextAlignment)this.GetValue(HorizontalTextAlignmentProperty);
        set => this.SetValue(HorizontalTextAlignmentProperty, value);
    }

    public double LineHeight
    {
        get => (double)this.GetValue(LineHeightProperty);
        set => this.SetValue(LineHeightProperty, value);
    }

    public double LetterSpacing
    {
        get => (double)this.GetValue(LetterSpacingProperty);
        set => this.SetValue(LetterSpacingProperty, value);
    }

    #endregion

    #region Constructors

    public GradientTextView()
    {
        this.InitializeComponent();
        this.AttachLinesCollection(this.Lines);
    }

    #endregion

    #region Overrides

    protected override Size MeasureOverride(double widthConstraint, double heightConstraint)
    {
        if (this.Lines == null || this.Lines.Count == 0)
        {
            return Size.Zero;
        }

        float scale = this._lastCanvasScale > 0 ? this._lastCanvasScale : this.GetScale();

        float canvasWidth = double.IsInfinity(widthConstraint)
            ? 100000f
            : (float)((widthConstraint - this.Margin.HorizontalThickness) * scale);

        float canvasHeight = double.IsInfinity(heightConstraint)
            ? 100000f
            : (float)(heightConstraint * scale);

        using var layout = this.BuildLayout(scale, canvasWidth, canvasHeight);
        if (layout == null)
        {
            return Size.Zero;
        }

        double logicalWidth = double.IsInfinity(widthConstraint) 
            ? layout.MaxLineWidth / scale 
            : widthConstraint;
        double logicalHeight = layout.TextHeight / scale;

        return new Size(logicalWidth, logicalHeight);
    }

    #endregion

    #region Event Handlers

    private static void OnVisualPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var ctrl = (GradientTextView)bindable;

        if (oldValue is INotifyCollectionChanged oldCol)
        {
            ctrl.DetachLinesCollection(oldCol);
        }

        if (newValue is INotifyCollectionChanged newCol)
        {
            ctrl.AttachLinesCollection(newCol);
        }

        ctrl.InvalidateMeasure();
        ctrl.canvasView?.InvalidateSurface();
    }

    private void LinesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        this.InvalidateMeasure();
        this.canvasView?.InvalidateSurface();
    }

    private void OnCanvasViewPaintSurface(object? sender, SKPaintSurfaceEventArgs e)
    {
        var canvas = e.Surface.Canvas;
        var info = e.Info;

        canvas.Clear(SKColors.Transparent);

        if (this.Lines == null || this.Lines.Count == 0)
        {
            return;
        }

        float deviceScale = this.GetScale();
        float canvasScale = this.Width > 0 ? info.Width / (float)this.Width : deviceScale;
        this._lastCanvasScale = canvasScale;

        using var layout = this.BuildLayout(canvasScale, info.Width, info.Height);
        if (layout == null)
        {
            return;
        }

        using var paint = new SKPaint
        {
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
            Shader = this.CreateShader(layout)
        };

        foreach (var (text, startX, baselineY, _) in layout.LinePositions)
        {
            this.DrawLineWithLetterSpacing(canvas, text, startX, baselineY, layout.Font, paint, layout.Spacing);
        }
    }

    #endregion

    #region Private Helpers

    private void AttachLinesCollection(object? collection)
    {
        if (collection is INotifyCollectionChanged col && !ReferenceEquals(col, this._attachedLinesCollection))
        {
            col.CollectionChanged += this.LinesCollectionChanged;
            this._attachedLinesCollection = col;
        }
    }

    private void DetachLinesCollection(INotifyCollectionChanged col)
    {
        if (ReferenceEquals(col, this._attachedLinesCollection))
        {
            col.CollectionChanged -= this.LinesCollectionChanged;
            this._attachedLinesCollection = null;
        }
    }

    private SKTypeface CreateTypeface()
    {
#if WINDOWS
        using var stream = FileSystem.Current.OpenAppPackageFileAsync("Metric-Medium.ttf").GetAwaiter().GetResult();
        return SKTypeface.FromStream(stream);
#else
        return SKTypeface.FromFamilyName("MetricMedium", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright);
#endif
    }

    private List<LayoutLine> BuildLayoutLines(SKFont font, float maxWidth, float spacing)
    {
        var result = new List<LayoutLine>();

        if (this.Lines == null || this.Lines.Count == 0)
        {
            return result;
        }

        float available = Math.Max(0, maxWidth);

        foreach (var raw in this.Lines)
        {
            if (string.IsNullOrEmpty(raw))
            {
                result.Add(new LayoutLine(string.Empty, 0));
                continue;
            }

            var words = raw.Split(' ');
            string currentLine = string.Empty;

            foreach (var word in words)
            {
                string candidate = string.IsNullOrEmpty(currentLine)
                    ? word
                    : currentLine + " " + word;

                float candidateWidth = this.MeasureTextWithLetterSpacing(candidate, font, spacing);
                if (candidateWidth <= available || string.IsNullOrEmpty(currentLine))
                {
                    currentLine = candidate;
                }
                else
                {
                    result.Add(new LayoutLine(currentLine, this.MeasureTextWithLetterSpacing(currentLine, font, spacing)));
                    currentLine = word;
                }
            }

            if (!string.IsNullOrEmpty(currentLine))
            {
                result.Add(new LayoutLine(currentLine, this.MeasureTextWithLetterSpacing(currentLine, font, spacing)));
            }
        }

        return result;
    }

    private float MeasureTextWithLetterSpacing(string text, SKFont font, float spacing)
    {
        float width = 0;
        for (int i = 0; i < text.Length; i++)
        {
            width += font.MeasureText(text[i].ToString());
            if (i < text.Length - 1)
            {
                width += spacing;
            }
        }
        return width;
    }

    private SKShader? CreateShader(LayoutContext layout)
    {
        var brush = this.Gradient;
        if (brush == null || brush.GradientStops.Count == 0 || layout.TextBoxWidth <= 0 || layout.TextBoxHeight <= 0)
        {
            return null;
        }

        var stops = brush.GradientStops.OrderBy(s => s.Offset).ToList();
        var colors = stops.Select(s => s.Color.ToSKColor()).ToArray();
        var positions = stops.Select(s => (float)s.Offset).ToArray();

        var start = new SKPoint(
            layout.TextBoxX + (float)brush.StartPoint.X * layout.TextBoxWidth,
            layout.TextBoxY + (float)brush.StartPoint.Y * layout.TextBoxHeight);

        var end = new SKPoint(
            layout.TextBoxX + (float)brush.EndPoint.X * layout.TextBoxWidth,
            layout.TextBoxY + (float)brush.EndPoint.Y * layout.TextBoxHeight);

        return SKShader.CreateLinearGradient(start, end, colors, positions, SKShaderTileMode.Clamp);
    }

    private void DrawLineWithLetterSpacing(SKCanvas canvas, string text, float startX, float baselineY, SKFont font, SKPaint paint, float spacing)
    {
        float x = startX;
        foreach (char c in text)
        {
            string ch = c.ToString();
            canvas.DrawText(ch, x, baselineY, font, paint);
            x += font.MeasureText(ch) + spacing;
        }
    }

    private float GetScale()
    {
        double density = DeviceDisplay.Current.MainDisplayInfo.Density;
        return (float)(density <= 0 ? 1 : density);
    }

    private LayoutContext? BuildLayout(float scale, float canvasWidth, float canvasHeight)
    {
        if (this.Lines == null || this.Lines.Count == 0)
        {
            return null;
        }

        var typeface = this.CreateTypeface();
        var font = new SKFont(typeface, (float)(this.FontSize * scale))
        {
            Edging = SKFontEdging.Antialias
        };

        float spacing = (float)(this.LetterSpacing * scale);
        var lines = this.BuildLayoutLines(font, canvasWidth, spacing);
        if (lines.Count == 0)
        {
            font.Dispose();
            return null;
        }

        var metrics = font.Metrics;
        float ascent = -metrics.Ascent;
        float descent = metrics.Descent;
        float capHeight = metrics.CapHeight > 0 ? metrics.CapHeight : ascent;
        float lineHeightMultiplier = (float)this.LineHeight;
        
        float rawLineHeight = ascent + descent + metrics.Leading;
        float lineHeight = rawLineHeight * lineHeightMultiplier;

        int lineCount = lines.Count;
        
        // Add platform-specific top padding to prevent text cutoff
        float topPadding = 0;
        if (Microsoft.Maui.Devices.DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.iOS || Microsoft.Maui.Devices.DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.WinUI)
        {
            topPadding = 3 * scale;
        }
        else if (Microsoft.Maui.Devices.DeviceInfo.Platform == Microsoft.Maui.Devices.DevicePlatform.MacCatalyst)
        {
            topPadding = 5 * scale;
        }
        
        // Tight text height: cap height for first line top, scaled descent for bottom
        float textHeight = capHeight + (lineCount - 1) * lineHeight + (descent * lineHeightMultiplier) + topPadding;

        float firstBaselineY = capHeight + topPadding;

        var linePositions = new List<(string text, float startX, float baselineY, float width)>();
        float minX = float.MaxValue;
        float maxX = float.MinValue;

        for (int i = 0; i < lines.Count; i++)
        {
            var (text, width) = lines[i];
            float baselineY = firstBaselineY + i * lineHeight;

            float startX = this.HorizontalTextAlignment switch
            {
                TextAlignment.Start => 0,
                TextAlignment.End => canvasWidth - width,
                _ => (canvasWidth - width) / 2f
            };

            linePositions.Add((text, startX, baselineY, width));
            minX = Math.Min(minX, startX);
            maxX = Math.Max(maxX, startX + width);
        }

        return new LayoutContext(
            typeface,
            font,
            linePositions,
            spacing,
            minX,
            topPadding,
            Math.Max(0, maxX - minX),
            textHeight,
            lines.Max(l => l.Width));
    }

    #endregion

    #region Nested Types

    private record LayoutLine(string Text, float Width);

    private sealed class LayoutContext(
        SKTypeface typeface,
        SKFont font,
        List<(string text, float startX, float baselineY, float width)> linePositions,
        float spacing,
        float textBoxX,
        float textBoxY,
        float textBoxWidth,
        float textBoxHeight,
        float maxLineWidth) : IDisposable
    {
        public SKFont Font { get; } = font;
        public List<(string text, float startX, float baselineY, float width)> LinePositions { get; } = linePositions;
        public float Spacing { get; } = spacing;
        public float TextBoxX { get; } = textBoxX;
        public float TextBoxY { get; } = textBoxY;
        public float TextBoxWidth { get; } = textBoxWidth;
        public float TextBoxHeight { get; } = textBoxHeight;
        public float TextHeight { get; } = textBoxHeight;
        public float MaxLineWidth { get; } = maxLineWidth;

        private readonly SKTypeface _typeface = typeface;

        public void Dispose()
        {
            this.Font?.Dispose();
            this._typeface?.Dispose();
        }
    }

    #endregion
}
