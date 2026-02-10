using MauiProgressRagDemo.ViewModels;

namespace MauiProgressRagDemo.Pages;

public partial class KnowledgeAssistantPage : PageBase
{
    public KnowledgeAssistantPage(KnowledgeAssistantViewModel viewModel)
    {
        this.InitializeComponent();
        this.BindingContext = viewModel;
    }
}