using Telerik.Maui.Controls;

namespace MauiProgressRagDemo.Behaviors;

public partial class SimultaneousTouchBehavior : Behavior<RadTemplatedButton>
{
    private RadTemplatedButton? button;

    protected override void OnAttachedTo(RadTemplatedButton bindable)
    {
        base.OnAttachedTo(bindable);
        this.button = bindable;

        this.button.HandlerChanged += this.OnHandlerChanged;
    }

    protected override void OnDetachingFrom(RadTemplatedButton bindable)
    {
        base.OnDetachingFrom(bindable);

        if (this.button != null)
        {
            this.button.HandlerChanged -= this.OnHandlerChanged;
            this.button = null;
        }
    }

    private void OnHandlerChanged(object? sender, EventArgs e)
    {
#if IOS
        this.Dispatcher.Dispatch(() =>
        {
            if (this.button?.Handler?.PlatformView is UIKit.UIView platformView)
            {
                var gestuerRecognizer = platformView.GestureRecognizers?.FirstOrDefault();
                if (gestuerRecognizer != null)
                {
                    gestuerRecognizer.ShouldRecognizeSimultaneously += (gestureRecognizer, otherGestureRecognizer) => true;
                }
            }
        });
#endif
    }
}
