using System.Collections;
using System.Windows.Input;
using Telerik.Maui.Controls;
using Telerik.Maui.ScrollView;

namespace MauiProgressRagDemo.Behaviors;

public class ChatScrollToBottomBehavior : Behavior<RadChat>
{
    public static readonly BindableProperty ScrollToBottomCommandProperty =
        BindableProperty.Create(nameof(ScrollToBottomCommand), typeof(ICommand), typeof(ChatScrollToBottomBehavior), defaultValue: null,
            propertyChanged: OnScrollToBottomCommandChanged);

    private RadChat? chat;
    private RadView? radScrollView2;
    private TaskCompletionSource<bool>? scrollCompletionSource;

    public ICommand? ScrollToBottomCommand
    {
        get => (ICommand?)GetValue(ScrollToBottomCommandProperty);
        set => SetValue(ScrollToBottomCommandProperty, value);
    }

    protected override void OnAttachedTo(RadChat bindable)
    {
        base.OnAttachedTo(bindable);

        this.chat = bindable;
        this.radScrollView2 = ChildrenOfType<RadView>(this.chat).FirstOrDefault();
    }

    protected override void OnDetachingFrom(RadChat bindable)
    {
        base.OnDetachingFrom(bindable);

        if (this.ScrollToBottomCommand is ChatScrollToBottomCommand scrollCommand)
        {
            scrollCommand.ExecuteRequested -= this.OnScrollRequested;
        }

        this.chat = null;
        this.radScrollView2 = null;
    }

    private static void OnScrollToBottomCommandChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (bindable is ChatScrollToBottomBehavior behavior)
        {
            // Unsubscribe from old command
            if (oldValue is ChatScrollToBottomCommand oldCommand)
            {
                oldCommand.ExecuteRequested -= behavior.OnScrollRequested;
            }

            // Subscribe to new command
            if (newValue is ChatScrollToBottomCommand newCommand)
            {
                newCommand.ExecuteRequested += behavior.OnScrollRequested;
            }
        }
    }

    private async void OnScrollRequested(object? sender, EventArgs e)
    {
        if (this.chat?.ItemsSource is IList items && items.Count > 0)
        {
            await this.ScrollToAsync(1000, false);
        }
    }

    private Task ScrollToAsync(double y, bool animated)
    {
        this.scrollCompletionSource = new TaskCompletionSource<bool>();
        var args = new ScrollToArgs(0, y, animated);
        this.radScrollView2?.Handler?.Invoke(nameof(IScrollView.RequestScrollTo), args);

        return this.scrollCompletionSource.Task;
    }

    private static IEnumerable<T> ChildrenOfType<T>(IVisualTreeElement element) where T : class
    {
        foreach (var child in element.GetVisualChildren())
        {
            if (child is T typedChild)
            {
                yield return typedChild;
            }

            if (child is IVisualTreeElement childElement)
            {
                foreach (var descendant in ChildrenOfType<T>(childElement))
                {
                    yield return descendant;
                }
            }
        }
    }
}

/// <summary>
/// Command implementation for scrolling to bottom of chat.
/// </summary>
public class ChatScrollToBottomCommand : ICommand
{
    public event EventHandler? CanExecuteChanged;
    public event EventHandler? ExecuteRequested;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        this.ExecuteRequested?.Invoke(this, EventArgs.Empty);
    }
}
