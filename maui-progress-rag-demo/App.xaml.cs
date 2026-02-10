#if WINDOWS
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Windowing;
using WinUIGrid = Microsoft.UI.Xaml.Controls.Grid;
using WinUIVerticalAlignment = Microsoft.UI.Xaml.VerticalAlignment;
using System.Net.Mime;
using WinRT.Interop;
using Microsoft.UI;
#endif

#if MACCATALYST
using UIKit;
using MauiProgressRagDemo.Platforms.MacCatalyst;
#endif

namespace MauiProgressRagDemo
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new AppShell());
#if WINDOWS
            window.HandlerChanged += (s, e) =>
            {
                if (window.Handler?.PlatformView is MauiWinUIWindow mauiWindow)
                {
                    var hWnd = WindowNative.GetWindowHandle(mauiWindow);
                    var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
                    var appWindow = AppWindow.GetFromWindowId(windowId);

                    appWindow.TitleBar.ExtendsContentIntoTitleBar = true;

                    var titleBlock = new TextBlock
                    {
                        Text = "Telerik UI for .Net Maui + Progress Agentic RAG",
                        VerticalAlignment = WinUIVerticalAlignment.Top,
                        HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                        Margin = new Microsoft.UI.Xaml.Thickness(0, 6, 0, 0),
                    };

                    var titleStack = new StackPanel
                    {
                        Orientation = Microsoft.UI.Xaml.Controls.Orientation.Horizontal,
                        VerticalAlignment = WinUIVerticalAlignment.Top,
                        HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Center,
                        Height = 48,
                    };

                    titleStack.Children.Add(titleBlock);

                    var titleHost = new WinUIGrid
                    {
                        Height = 48,
                        VerticalAlignment = WinUIVerticalAlignment.Top,
                        HorizontalAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Stretch,
                    };

                    titleHost.Children.Add(titleStack);

                    if (mauiWindow.Content is Microsoft.UI.Xaml.Controls.Panel root)
                    {
                        root.Children.Add(titleHost);
                    }

                    mauiWindow.SetTitleBar(titleHost);
                }
            };
#endif

#if MACCATALYST
            window.HandlerChanged += (s, e) =>
            {
                if (window.Handler?.PlatformView is UIWindow uiWindow)
                {
                    WindowHelper.SetupCenteredTitlebar(uiWindow, "Telerik UI for .NET MAUI + Progress Agentic RAG");
                }
            };
#endif

            return window;
        }
    }
}