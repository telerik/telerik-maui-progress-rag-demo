using MauiProgressRagDemo.ViewModels;

namespace MauiProgressRagDemo.Pages;

public partial class IntelligentSearchPage : PageBase
{
    public IntelligentSearchPage(IntelligentSearchViewModel viewModel)
    {
        this.InitializeComponent();
        this.BindingContext = viewModel;
    }
}