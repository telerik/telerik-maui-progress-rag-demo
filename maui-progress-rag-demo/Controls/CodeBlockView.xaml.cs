using Telerik.Maui.Controls;

namespace MauiProgressRagDemo.Controls;

public partial class CodeBlockView : ContentView
{
    public static readonly BindableProperty CodeProperty =
        BindableProperty.Create(nameof(Code), typeof(string), typeof(CodeBlockView), string.Empty,
            propertyChanged: OnCodeChanged);

    public CodeBlockView()
    {
        this.InitializeComponent();
    }

    public string Code
    {
        get => (string)this.GetValue(CodeProperty);
        set => this.SetValue(CodeProperty, value);
    }

    private static void OnCodeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is CodeBlockView codeBlockView && newValue is string code)
        {
            codeBlockView.CodeLabel.Text = code;
        }
    }

    private async void OnCopyClicked(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.Code))
        {
            await Clipboard.SetTextAsync(this.Code);
            
            if (sender is RadTemplatedButton button && (button.Content == null))
            {
                var originalText = button.Content;
                button.Content += "   Copied!";
                await Task.Delay(2000);
                button.Content = originalText;
            }
        }
    }
}
