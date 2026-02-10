using MauiProgressRagDemo.Models;
using MauiProgressRagDemo.Services;

namespace MauiProgressRagDemo.ViewModels;

public class HomeViewModel : ChatViewModelBase
{
    public HomeViewModel(NucliaSearchService nucliaSearchService) : base(nucliaSearchService)
    {
    }

    public List<NavigationItem> NavigationItems { get; } = new List<NavigationItem>
    {
        new() { Title = "Intelligent Search", Icon = "scansearch.png", Route = "IntelligentSearch" },
        new() { Title = "Financial Analysis", Icon = "chartarea.png", Route = "FinancialAnalysis" },
        new() { Title = "Knowledge Assistant", Icon = "bot.png", Route = "KnowledgeAssistant" },
        new() { Title = "Agentic RAG Value", Icon = "sparkles.png", Route = "AgenticRagValue" }
    };

    protected override Task SendMessageAsync(object param)
    {
        var query = this.CurrentMessage;
        if (string.IsNullOrWhiteSpace(query))
        {
            return Task.CompletedTask;
        }

        return Shell.Current.GoToAsync($"///IntelligentSearch?query={Uri.EscapeDataString(query)}");
    }
}