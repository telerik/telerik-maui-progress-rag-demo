using Telerik.Maui.Controls;

namespace MauiProgressRagDemo.Behaviors;

/// <summary>
/// Behavior that resets RadTemplatedButton control template when clicked to clear pointer over state.
/// This is only needed on Windows to clear the PointerOver state after navigation.
/// </summary>
public class ResetTemplateOnClickBehavior : Behavior<RadTemplatedButton>
{
    private RadTemplatedButton? button;

    protected override void OnAttachedTo(RadTemplatedButton bindable)
    {
        base.OnAttachedTo(bindable);

        this.button = bindable;
        this.button.Clicked += this.OnButtonClicked;
    }

    protected override void OnDetachingFrom(RadTemplatedButton bindable)
    {
        base.OnDetachingFrom(bindable);

        if (this.button != null)
        {
            this.button.Clicked -= this.OnButtonClicked;
            this.button = null;
        }
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
#if WINDOWS
        if (this.button != null)
        {
            var template = this.button.ControlTemplate;
            this.button.ControlTemplate = null;
            this.button.ControlTemplate = template;
        }
#endif
    }
}
