using UIKit;
using CoreFoundation;

namespace MauiProgressRagDemo.Platforms.MacCatalyst
{
    public static class WindowHelper
    {
        public static void SetupCenteredTitlebar(UIWindow uiWindow, string title)
        {
            if (uiWindow?.WindowScene == null)
                return;

            var titlebar = uiWindow.WindowScene.Titlebar;
            if (titlebar == null)
                return;

            // Hide the default title
            titlebar.TitleVisibility = UITitlebarTitleVisibility.Hidden;
            
            // Wait for the window to be fully loaded
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                SetupTitlebarLabel(uiWindow, title);
            });
        }

        private static void SetupTitlebarLabel(UIWindow uiWindow, string title)
        {
            if (uiWindow.RootViewController?.View == null)
                return;

            var rootView = uiWindow.RootViewController.View;
            
            // Remove any existing titlebar label
            foreach (var subview in rootView.Subviews)
            {
                if (subview is UIView view && view.Tag == 99999)
                {
                    view.RemoveFromSuperview();
                }
            }

            // Create a container view for the titlebar
            var titlebarContainer = new UIView
            {
                Tag = 99999,
                BackgroundColor = UIColor.Clear,
                TranslatesAutoresizingMaskIntoConstraints = false
            };

            // Create the centered label
            var titleLabel = new UILabel
            {
                Text = title,
                TextAlignment = UITextAlignment.Center,
                Font = UIFont.SystemFontOfSize(16, UIFontWeight.Semibold),
                TextColor = UIColor.Label,
                TranslatesAutoresizingMaskIntoConstraints = false
            };
            
            titlebarContainer.AddSubview(titleLabel);
            rootView.AddSubview(titlebarContainer);

            // Add constraints to position and size the titlebar
            NSLayoutConstraint.ActivateConstraints(new[]
            {
                titlebarContainer.TopAnchor.ConstraintEqualTo(rootView.TopAnchor),
                titlebarContainer.LeadingAnchor.ConstraintEqualTo(rootView.LeadingAnchor),
                titlebarContainer.TrailingAnchor.ConstraintEqualTo(rootView.TrailingAnchor),
                titlebarContainer.HeightAnchor.ConstraintEqualTo(52),
                
                titleLabel.CenterXAnchor.ConstraintEqualTo(titlebarContainer.CenterXAnchor),
                titleLabel.CenterYAnchor.ConstraintEqualTo(titlebarContainer.CenterYAnchor),
                titleLabel.LeadingAnchor.ConstraintGreaterThanOrEqualTo(titlebarContainer.LeadingAnchor, 80),
                titleLabel.TrailingAnchor.ConstraintLessThanOrEqualTo(titlebarContainer.TrailingAnchor, -80)
            });
        }
    }
}
