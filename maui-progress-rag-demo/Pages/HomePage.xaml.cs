using MauiProgressRagDemo.ViewModels;

namespace MauiProgressRagDemo.Pages;

public partial class HomePage : PageBase
{
    public HomePage(HomeViewModel viewModel)
    {
        this.InitializeComponent();
        this.BindingContext = viewModel;
    }
}