#if IOS
using CoreGraphics;
using Foundation;
using UIKit;

namespace MauiProgressRagDemo.Services;

partial class ToastMessageService 
{
    private NSTimer alertDelay;
    private UIAlertController alert;

    public void ShortAlert(string message)
    {
        this.ShowAlert(message, 2.0);
    }

    private void ShowAlert(string message, double seconds)
    {
        this.alertDelay = NSTimer.CreateScheduledTimer(seconds, (obj) =>
        {
            this.DismissMessage();
        });

        this.alert = UIAlertController.Create(null, message, UIAlertControllerStyle.ActionSheet);

        var rootViewController = UIApplication.SharedApplication.KeyWindow.RootViewController;
        var popoverPresentationController = this.alert.PopoverPresentationController;

        if (popoverPresentationController != null)
        {
            var rootView = rootViewController.View;
            var rootRect = rootViewController.View.Bounds;

            var sourceRect = new CGRect(rootRect.Width / 2, 180, 0, 0);

            popoverPresentationController.SourceView = rootView;
            popoverPresentationController.SourceRect = sourceRect;
            popoverPresentationController.PermittedArrowDirections = 0; // No arrow
        }

        rootViewController.PresentViewController(this.alert, true, null);
    }

    private void DismissMessage()
    {
        if (this.alert != null)
        {
            this.alert.DismissViewController(true, null);
        }

        if (this.alertDelay != null)
        {
            this.alertDelay.Dispose();
        }
    }
}
#endif