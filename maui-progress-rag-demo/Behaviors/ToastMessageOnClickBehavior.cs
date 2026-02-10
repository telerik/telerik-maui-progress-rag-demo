using MauiProgressRagDemo.Services;
using Telerik.Maui.Controls;

namespace MauiProgressRagDemo.Behaviors;

/// <summary>
/// Behavior that shows a toast message with the tooltip text when a button is clicked or navigation item is clicked.
/// Supports RadTemplatedButton.Clicked and RadNavigationView.ItemClicked events.
/// </summary>
public class ToastMessageOnClickBehavior : Behavior<View>
{
    private View? attachedElement;
    private IToastMessageService? toastMessageService;
    private readonly string ToastText = "Interaction is disabled for this demo.";

    protected override void OnAttachedTo(View bindable)
    {
        base.OnAttachedTo(bindable);

        this.attachedElement = bindable;

        // Get the toast service from the service provider
        this.toastMessageService = Application.Current?.Handler?.MauiContext?.Services.GetService<IToastMessageService>();

        // Subscribe to appropriate event based on control type
        if (bindable is RadTemplatedButton button)
        {
            button.Clicked += this.OnButtonClicked;
        }
        else if (bindable is RadNavigationView navigationView)
        {
            navigationView.ItemClicked += this.OnNavigationItemClicked;
        }
    }

    protected override void OnDetachingFrom(View bindable)
    {
        base.OnDetachingFrom(bindable);

        // Unsubscribe from events based on control type
        if (bindable is RadTemplatedButton button)
        {
            button.Clicked -= this.OnButtonClicked;
        }
        else if (bindable is RadNavigationView navigationView)
        {
            navigationView.ItemClicked -= this.OnNavigationItemClicked;
        }

        this.attachedElement = null;
        this.toastMessageService = null;
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        this.toastMessageService?.ShortAlert(this.ToastText);
    }

    private void OnNavigationItemClicked(object? sender, object e)
    {
        this.toastMessageService?.ShortAlert(this.ToastText);
    }
}
