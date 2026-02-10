using MauiProgressRagDemo.Services;
using System.Collections.ObjectModel;

namespace MauiProgressRagDemo.ViewModels;

public class AgenticRagValueViewModel : ViewModelBase
{
    private string? selectedIndustry;
    private string? selectedCompanySize;
    private string? selectedUseCase;
    private string? additionalDetails;
    private string valueProposition;
    private bool hasResults;

    public AgenticRagValueViewModel(NucliaSearchService nucliaSearchService) : base(nucliaSearchService)
    {
    }

    public List<string> Industries { get; } = new()
    {
        "Financial Services", 
        "Healthcare", 
        "Manufacturing", 
        "Technology",
        "Retail & E-commerce", 
        "Energy & Utilities", 
        "Government", 
        "Education",
        "Professional Services", 
        "Media & Entertainment", 
        "Transportation & Logistics",
        "Real Estate", 
        "Other"
    };

    public List<string> CompanySizes { get; } = new()
    {
        "Small Business (1-100 employees)", 
        "Mid-Market (101-1,000 employees)",
        "Large Enterprise (1,001-10,000 employees)", 
        "Global Enterprise (10,000+ employees)"
    };

    public List<string> DataTypes { get; } = new()
    {
        "Customer Data", 
        "Product Documentation", 
        "Internal Knowledge Base",
        "Compliance Documents", 
        "Training Materials", 
        "Technical Documentation",
        "Sales & Marketing Content", 
        "HR Policies & Procedures", 
        "Financial Reports",
        "Research & Development", 
        "Legal Documents", 
        "Operational Procedures"
    };

    public List<string> UseCases { get; } = new()
    {
        "Customer Support Enhancement", 
        "Employee Self-Service",
        "Sales Enablement", 
        "Compliance & Risk Management",
        "Research & Development", 
        "Knowledge Management",
        "Process Automation", 
        "Decision Support",
        "Content Management", 
        "Other"
    };

    public List<string> Results { get; } = new()
    {
        "UseCases",
        "DataTypes",
        "CompanySizes"
    };

    public string? SelectedIndustry
    {
        get => this.selectedIndustry;
        set
        {
            this.selectedIndustry = value;
            this.OnPropertyChanged();
        }        
    }

    public string? SelectedCompanySize
    { 
        get => this.selectedCompanySize;
        set
        {
            this.selectedCompanySize = value;
            this.OnPropertyChanged();
        }
    }

    public string? SelectedUseCase
    {
        get => this.selectedUseCase;
        set
        {
            this.selectedUseCase = value;
            this.OnPropertyChanged();
        }    
    }

    public string? AdditionalDetails
    {
        get => this.additionalDetails;
        set
        {
            this.additionalDetails = value;
            this.OnPropertyChanged();
        }
    }

    public bool HasResults
    {
        get => this.hasResults;
        set
        {
            this.hasResults = value;
            this.OnPropertyChanged(nameof(SelectedDataTypesString));
            this.OnPropertyChanged();
        }
    }

    public string ValueProposition
    {
        get => this.valueProposition;
        set
        {
            this.valueProposition = value;
            this.OnPropertyChanged();
        }
    }

    public ObservableCollection<object> SelectedDataTypes { get; set; }
    public string SelectedDataTypesString => string.Join(", ", this.SelectedDataTypes ?? Enumerable.Empty<object>());
    public Command GenerateValuePropositionCommand => new Command(async () => await HandleGenerate());

    public override void ResetState()
    {
        base.ResetState();
        this.SelectedIndustry = null;
        this.SelectedCompanySize = null;
        this.SelectedUseCase = null;
        this.AdditionalDetails = null;
        this.ValueProposition = string.Empty;
        this.HasResults = false;
        this.SelectedDataTypes?.Clear();
    }

    private string GenerateQuestion()
    {
        var parts = new List<string>();
        parts.Add("Generate a compelling value proposition for Progress Agentic RAG and Telerik DevTools");

        if (!string.IsNullOrEmpty(this.SelectedIndustry))
        {
            parts.Add($"for a company in the {this.SelectedIndustry} industry");
        }

        if (!string.IsNullOrEmpty(this.SelectedCompanySize))
        {
            parts.Add($"with {this.SelectedCompanySize} employees");
        }

        if (SelectedDataTypes.Count > 0)
        {
            parts.Add($"that works with {this.SelectedDataTypesString}");
        }

        if (!string.IsNullOrEmpty(this.SelectedUseCase))
        {
            parts.Add($"focusing on {this.SelectedUseCase}");
        }

        if (!string.IsNullOrEmpty(this.AdditionalDetails))
        {
            parts.Add($"Additional context: {this.AdditionalDetails}");
        }

        parts.Add("Highlight the benefits of combining AI-powered search with enterprise-grade UI components.");

        return string.Join(" ", parts);
    }

    private async Task HandleGenerate()
    {
        var question = GenerateQuestion();

        this.IsLoading = true;
        this.HasResults = true; 
        this.ValueProposition = string.Empty;

        try
        {
            await NucliaService.AskVerseAsync(question, (chunk) =>
            {
                this.ValueProposition = chunk;
            });
        }
        catch (Exception ex)
        {
            this.ValueProposition = $"Sorry, I encountered an error: {ex.Message}";
        }
        finally
        {
            this.IsLoading = false;
        }
    }
}