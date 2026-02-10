using System.Text;
using System.Text.RegularExpressions;

namespace MauiProgressRagDemo.Controls;

public partial class MarkdownView : ContentView
{
    public static readonly BindableProperty FontFamilyProperty =
        BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(MarkdownView), "MericRegular",
            propertyChanged: OnAppearanceChanged);

    public static readonly BindableProperty FontSizeProperty =
        BindableProperty.Create(nameof(FontSize), typeof(double), typeof(MarkdownView), 16.0,
            propertyChanged: OnAppearanceChanged);

    public static readonly BindableProperty TextColorProperty =
        BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(MarkdownView), Color.FromArgb("#000000"),
            propertyChanged: OnAppearanceChanged);

    public static readonly BindableProperty TextProperty =
        BindableProperty.Create(nameof(Text), typeof(string), typeof(MarkdownView), string.Empty,
            propertyChanged: OnTextChanged);

    public MarkdownView()
    {
        InitializeComponent();
    }

    public string FontFamily
    {
        get => (string)GetValue(FontFamilyProperty);
        set => SetValue(FontFamilyProperty, value);
    }

    public double FontSize
    {
        get => (double)GetValue(FontSizeProperty);
        set => SetValue(FontSizeProperty, value);
    }

    public Color TextColor
    {
        get => (Color)GetValue(TextColorProperty);
        set => SetValue(TextColorProperty, value);
    }

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MarkdownView markdownView)
        {
            markdownView.RenderMarkdown();
        }
    }

    private static void OnAppearanceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is MarkdownView markdownView)
        {
            markdownView.RenderMarkdown();
        }
    }

    private void RenderMarkdown()
    {
        if (string.IsNullOrEmpty(Text))
        {
            ContentStack.Clear();
            return;
        }

        var bodyFontSize = FontSize;
        var bodyFontFamily = FontFamily;
        var bodyTextColor = TextColor;

        var lines = Text.Split('\n');
        var currentCodeBlock = new List<string>();
        var inCodeBlock = false;
        var currentList = new List<(string Content, bool IsOrdered, int? Number)>();

        var renderedElements = new List<View>();

        void FlushList()
        {
            if (currentList.Count == 0)
                return;

            var isOrdered = currentList[0].IsOrdered;

            // Try to reuse existing list container
            VerticalStackLayout listContainer;
            var currentIndex = renderedElements.Count;
            if (currentIndex < ContentStack.Count && ContentStack[currentIndex] is VerticalStackLayout existingList)
            {
                listContainer = existingList;
                listContainer.Clear();
            }
            else
            {
                listContainer = new VerticalStackLayout();
            }

            listContainer.Spacing = 4;
            listContainer.Margin = new Thickness(24, 0, 0, 8);

            for (int i = 0; i < currentList.Count; i++)
            {
                var item = currentList[i];
                var itemLayout = new Grid
                {
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = new GridLength(20, GridUnitType.Absolute) },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                    },
                    ColumnSpacing = 8,
                    Margin = new Thickness(0, 0, 0, i == currentList.Count - 1 ? 0 : 4)
                };

                var bulletText = isOrdered
                    ? $"{(item.Number ?? i + 1)}."
                    : "\u2022";

                var bullet = new Label
                {
                    Text = bulletText,
                    FontSize = bodyFontSize,
                    FontFamily = bodyFontFamily,
                    TextColor = bodyTextColor,
                    VerticalOptions = LayoutOptions.Start,
                    Margin = new Thickness(0, 2, 0, 0)
                };

                var contentLabel = new Label
                {
                    FormattedText = ProcessInlineMarkdown(item.Content),
                    FontSize = bodyFontSize,
                    FontFamily = bodyFontFamily,
                    TextColor = bodyTextColor,
                    LineHeight = 1.2,
                    LineBreakMode = LineBreakMode.WordWrap
                };

                itemLayout.Add(bullet);
                Grid.SetColumn(bullet, 0);
                
                itemLayout.Add(contentLabel);
                Grid.SetColumn(contentLabel, 1);
                
                listContainer.Add(itemLayout);
            }

            renderedElements.Add(listContainer);
            currentList.Clear();
        }

        Label CreateOrUpdateLabel(
            double fontSize,
            string fontFamily,
            Color textColor,
            FormattedString formattedText,
            Thickness margin,
            FontAttributes fontAttributes)
        {
            Label label;
            var currentIndex = renderedElements.Count;

            if (currentIndex < ContentStack.Count && ContentStack[currentIndex] is Label existingLabel)
            {
                label = existingLabel;
            }
            else
            {
                label = new Label
                {
                    LineBreakMode = LineBreakMode.WordWrap,
                    HorizontalOptions = LayoutOptions.Fill
                };
            }

            label.FormattedText = formattedText;
            label.FontSize = fontSize;
            label.FontFamily = fontFamily;
            label.TextColor = textColor;
            label.Margin = margin;
            label.FontAttributes = fontAttributes;
            label.HorizontalOptions = LayoutOptions.Fill;

            return label;
        }

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();

            if (trimmedLine.StartsWith("```"))
            {
                FlushList();
                if (!inCodeBlock)
                {
                    inCodeBlock = true;
                }
                else
                {
                    inCodeBlock = false;
                    var code = string.Join("\n", currentCodeBlock);

                    // Try to reuse existing CodeBlockView
                    CodeBlockView codeBlock;
                    var currentIndex = renderedElements.Count;
                    if (currentIndex < ContentStack.Count && ContentStack[currentIndex] is CodeBlockView existingCodeBlock)
                    {
                        codeBlock = existingCodeBlock;
                        codeBlock.Code = code;
                        codeBlock.Margin = new Thickness(0, 0, 0, 8);
                    }
                    else
                    {
                        codeBlock = new CodeBlockView
                        {
                            Code = code,
                            Margin = new Thickness(0, 0, 0, 8)
                        };
                    }

                    renderedElements.Add(codeBlock);
                    currentCodeBlock.Clear();
                }
                continue;
            }

            if (inCodeBlock)
            {
                currentCodeBlock.Add(line);
                continue;
            }

            if (line.StartsWith("### "))
            {
                FlushList();
                var content = line[4..];
                var heading = CreateOrUpdateLabel(
                    bodyFontSize * 1.2,
                    bodyFontFamily,
                    bodyTextColor,
                    ProcessInlineMarkdown(content),
                    new Thickness(0, 16, 0, 8),
                    FontAttributes.Bold);
                renderedElements.Add(heading);
            }
            else if (line.StartsWith("## "))
            {
                FlushList();
                var content = line[3..];
                var heading = CreateOrUpdateLabel(
                    bodyFontSize * 1.35,
                    bodyFontFamily,
                    bodyTextColor,
                    ProcessInlineMarkdown(content),
                    new Thickness(0, 18, 0, 10),
                    FontAttributes.Bold);
                renderedElements.Add(heading);
            }
            else if (line.StartsWith("# "))
            {
                FlushList();
                var content = line[2..];
                var heading = CreateOrUpdateLabel(
                    bodyFontSize * 1.5,
                    bodyFontFamily,
                    bodyTextColor,
                    ProcessInlineMarkdown(content),
                    new Thickness(0, 20, 0, 12),
                    FontAttributes.Bold);
                renderedElements.Add(heading);
            }
            else if (Regex.IsMatch(trimmedLine, @"^[-*+]\s"))
            {
                var content = trimmedLine[2..];
                currentList.Add((content, false, null));
            }
            else if (Regex.IsMatch(trimmedLine, @"^\d+\.\s"))
            {
                var match = Regex.Match(trimmedLine, @"^(?<num>\d+)\.\s");
                int? number = null;
                if (match.Success && int.TryParse(match.Groups["num"].Value, out var parsed))
                {
                    number = parsed;
                }

                var content = Regex.Replace(trimmedLine, @"^\d+\.\s", "");
                currentList.Add((content, true, number));
            }
            else if (string.IsNullOrWhiteSpace(trimmedLine))
            {
                FlushList();
            }
            else
            {
                FlushList();
                var paragraph = CreateOrUpdateLabel(
                    bodyFontSize,
                    bodyFontFamily,
                    bodyTextColor,
                    ProcessInlineMarkdown(line),
                    new Thickness(0, 0, 0, 8),
                    FontAttributes.None);
                renderedElements.Add(paragraph);
            }
        }

        FlushList();

        // Update ContentStack with rendered elements
        // Remove extra children if we have fewer elements now
        while (ContentStack.Count > renderedElements.Count)
        {
            ContentStack.RemoveAt(ContentStack.Count - 1);
        }

        // Update or add elements
        for (int i = 0; i < renderedElements.Count; i++)
        {
            if (i < ContentStack.Count)
            {
                if (ContentStack[i] != renderedElements[i])
                {
                    ContentStack.RemoveAt(i);
                    ContentStack.Insert(i, renderedElements[i]);
                }
            }
            else
            {
                ContentStack.Add(renderedElements[i]);
            }
        }
    }

    private FormattedString ProcessInlineMarkdown(string text)
    {
        if (string.IsNullOrEmpty(text))
            return new FormattedString();

        var formattedString = new FormattedString();
        var regex = new Regex(@"(\*\*([^*]+)\*\*)|(\*([^*]+)\*)|(`([^`]+)`)|\[([^\]]+)\]\(([^)]+)\)");
        var lastIndex = 0;

        foreach (Match match in regex.Matches(text))
        {
            // Add plain text before the match
            if (match.Index > lastIndex)
            {
                formattedString.Spans.Add(new Span
                {
                    Text = text[lastIndex..match.Index]
                });
            }

            // Bold text **text**
            if (!string.IsNullOrEmpty(match.Groups[1].Value))
            {
                formattedString.Spans.Add(new Span
                {
                    Text = match.Groups[2].Value,
                    FontAttributes = FontAttributes.Bold
                });
            }
            // Italic text *text*
            else if (!string.IsNullOrEmpty(match.Groups[3].Value))
            {
                formattedString.Spans.Add(new Span
                {
                    Text = match.Groups[4].Value,
                    FontAttributes = FontAttributes.Italic
                });
            }
            // Inline code `code`
            else if (!string.IsNullOrEmpty(match.Groups[5].Value))
            {
                formattedString.Spans.Add(new Span
                {
                    Text = match.Groups[6].Value,
                    BackgroundColor = Color.FromArgb("#f5f5f5"),
                    FontFamily = "Courier New",
                    FontSize = 13
                });
            }
            // Links [text](url)
            else if (!string.IsNullOrEmpty(match.Groups[7].Value))
            {
                var linkSpan = new Span
                {
                    Text = match.Groups[7].Value,
                    TextColor = Color.FromArgb("#0066cc"),
                    TextDecorations = TextDecorations.Underline
                };

                // Note: MAUI doesn't support clickable spans directly
                // You would need to implement a TapGestureRecognizer on the Label if needed
                formattedString.Spans.Add(linkSpan);
            }

            lastIndex = match.Index + match.Length;
        }

        // Add remaining text
        if (lastIndex < text.Length)
        {
            formattedString.Spans.Add(new Span
            {
                Text = text[lastIndex..]
            });
        }

        return formattedString.Spans.Count > 0 ? formattedString : new FormattedString
        {
            Spans = { new Span { Text = text } }
        };
    }
}
