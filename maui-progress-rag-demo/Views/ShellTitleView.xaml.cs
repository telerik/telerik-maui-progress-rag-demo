using MauiProgressRagDemo.ViewModels;

namespace MauiProgressRagDemo.Views;

public partial class ShellTitleView : ContentView
{
    public ShellTitleView()
    {
        this.InitializeComponent();
        this.BindingContext = new ViewModelBase(null);
    }
}