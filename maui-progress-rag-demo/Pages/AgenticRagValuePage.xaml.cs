using MauiProgressRagDemo.ViewModels;

namespace MauiProgressRagDemo.Pages;

public partial class AgenticRagValuePage : PageBase
{
    public AgenticRagValuePage(AgenticRagValueViewModel viewModel)
    {
        this.InitializeComponent();
        this.BindingContext = viewModel;
    }
}