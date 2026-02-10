using MauiProgressRagDemo.Services;
using System.Windows.Input;

namespace MauiProgressRagDemo.ViewModels;

[QueryProperty(nameof(QueryFromNavigation), "query")]
public class IntelligentSearchViewModel : ViewModelBase
{
    private string query = string.Empty;
    private string answer = string.Empty;

    public IntelligentSearchViewModel(NucliaSearchService nucliaSearchService) : base(nucliaSearchService)
    {
    }

    public string QueryFromNavigation
    {
        set
        {
            var unescapedValue = Uri.UnescapeDataString(value ?? string.Empty);
            _ = HandleSearch(unescapedValue);
        }
    }

    public string Query
    {
        get => this.query;
        set
        {
            if (this.query != value)
            {
                this.query = value;
                this.OnPropertyChanged();
            }
        }
    }

    public string Answer
    {
        get => this.answer;
        set
        {
            if (this.answer != value)
            {
                this.answer = value;
                this.OnPropertyChanged();
                this.OnPropertyChanged(nameof(HasResults));
            }
        }
    }

    public ICommand SearchCommand => new Command(async (object param) => await HandleSearch(param));
    public bool HasResults => !string.IsNullOrEmpty(Answer) || IsLoading;

    public List<string> PredefinedSearches { get; } = new List<string>
    {
        "What is PARAG and how does it work?",
        "Deployment options and requirements",
        "Security features and compliance",
        "API integration and capabilities",
        "Pricing and licensing options",
        "Use cases and customer success stories"
    };

    public override void ResetState()
    {
        base.ResetState();
        this.Query = string.Empty;
        this.Answer = string.Empty;
    }

    private async Task HandleSearch(object param)
    {
        var query = param as string;
        if (!string.IsNullOrWhiteSpace(query))
        {
            this.Query = query;
        }

        if (string.IsNullOrWhiteSpace(this.Query) || this.IsLoading)
        {
            return;
        }

        this.IsLoading = true;
        this.Answer = string.Empty;
        string tempAnswer = string.Empty;
        this.OnPropertyChanged(nameof(this.HasResults));

        try
        {
            await this.NucliaService.AskVerseAsync(this.Query, (partialResponse) =>
            {
                tempAnswer = partialResponse;
            });
        }
        catch (Exception ex)
        {
            this.Answer = $"Sorry, I encountered an error: {ex.Message}";
        }
        finally
        {
            this.Answer = tempAnswer;
            this.IsLoading = false;
        }
    }
}