using Telerik.Maui;
using Telerik.Maui.Controls;
using Telerik.Maui.Controls.Paths;

namespace MauiProgressRagDemo.Behaviors;

public class BusyIndicatorAnimationBehavior : Behavior<RadBusyIndicator>
{
    private RadBusyIndicator? indicator;

    protected override void OnAttachedTo(RadBusyIndicator bindable)
    {
        base.OnAttachedTo(bindable);
        this.indicator = bindable;
        this.SetupCustomAnimation();
    }

    protected override void OnDetachingFrom(RadBusyIndicator bindable)
    {
        base.OnDetachingFrom(bindable);
        this.indicator = null;
    }

    private void SetupCustomAnimation()
    {
        if (this.indicator == null)
        {
            return;
        }

        RadPathFigure figure = new RadPathFigure();
        figure.StartPoint = new Point(0.5, 0.5);

        // Outer circle
        RadArcSegment outer = new RadArcSegment();
        outer.Center = new Point(0.5, 0.5);
        outer.Size = new Size(1, 1);
        outer.StartAngle = 0;
        outer.SweepAngle = -360;
        figure.Segments.Add(outer);

        // Inner circle to create a ring (donut)
        RadArcSegment inner = new RadArcSegment();
        inner.Center = new Point(0.5, 0.5);
        inner.Size = new Size(0.85, 0.85);
        inner.StartAngle = 0;
        inner.SweepAngle = 360;
        figure.Segments.Add(inner);

        RadPathGeometry geometry = new RadPathGeometry();
        geometry.Figures.Add(figure);
        geometry.Transform = new RadRotateTransform(0.5, 0.5, 0);

        var brush = new RadSweepGradientBrush(new Point(0.5, 0.5));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF96E5FF"), 0));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF6840FF"), 30));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF5845FF"), 60));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF4C50FF"), 90));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF4860FF"), 120));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF4570FF"), 150));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF4580FF"), 180));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF4570FF"), 210));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF4798FF"), 240));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF5DBDFF"), 270));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF70C8FF"), 300));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF82DCFF"), 330));
        brush.GradientStops.Add(new RadSweepGradientStop(Color.FromArgb("FF96E5FF"), 360));

        var path = new RadPath();
        path.WidthRequest = 80;
        path.HeightRequest = 80;
        path.Fill = brush;
        path.Geometry = geometry;
        this.indicator.BusyContent = path;

        var animation = new RadDoubleAnimation
        {
            Duration = 1000,
            Easing = Easing.Linear,
            From = 0,
            To = -360,
            RepeatForever = true,
            Target = geometry,
            PropertyPath = "Transform.Angle"
        };

        this.indicator.Animations.Add(animation);
        this.indicator.AnimationType = AnimationType.Custom;
    }
}
