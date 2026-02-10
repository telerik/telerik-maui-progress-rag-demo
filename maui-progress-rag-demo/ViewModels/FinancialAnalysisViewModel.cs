using MauiProgressRagDemo.Models;
using MauiProgressRagDemo.Services;
using System.Collections.ObjectModel;
using Telerik.Maui.Controls.Chat;

namespace MauiProgressRagDemo.ViewModels;

public class FinancialAnalysisViewModel : ChatViewModelBase
{
    private List<TelerikChartModel> selectedCharts;
    private bool shouldShowCharts;

    public FinancialAnalysisViewModel(NucliaSearchService nucliaSearchService) : base(nucliaSearchService)
    {
        this.SuggestedChatMessages = new ObservableCollection<string>
        {
            "Compare Nvidia and Google revenue?",
            "Product line contribution to Apple revenue?",
            "Google financial results 2024 vs 2023?"
        };

        this.ToggleShouldShowChartsCommand = new Command(() =>
        {
            this.ShouldShowCharts = !this.ShouldShowCharts;
        });
    }

    public List<TelerikChartModel> SelectedCharts
    {
        get => this.selectedCharts;
        set
        {
            this.selectedCharts = value;
            this.OnPropertyChanged();
        }
    }

    public bool ShouldShowCharts
    {
        get => this.shouldShowCharts;
        set
        {
            this.shouldShowCharts = value;
            this.OnPropertyChanged();
        }
    }

    public Command ToggleShouldShowChartsCommand { get; private set; }

    public override void ResetState()
    {
        base.ResetState();
        this.SelectedCharts = null;
        this.ShouldShowCharts = false;
    }

    protected async override Task SendMessageAsync(object param)
    {
        var messageText = param as string ?? this.CurrentMessage;
        if (string.IsNullOrWhiteSpace(messageText))
        {
            return;
        }

        if (this.isInitialMessage)
        {
            this.isInitialMessage = false;
            this.ChatTitle = messageText;
            this.HasConversationStarted = true;

            var userMessage = new TextMessage
            {
                Author = this.UserAuthor,
                Text = messageText
            };

            this.ChatMessages.Add(userMessage);
        }

        this.IsLoading = true;
        this.ShowTypingIndicator(this.BotAuthor);

        try
        {
            ChartAugmentedAnswer? answer = await this.NucliaService.AskChartsAsync(messageText);
            if (answer != null)
            {
                this.SendBotMessage(answer.Answer);

                // Update selected charts
                if (answer.Charts != null && answer.Charts.Any())
                {
                    this.SendBotChartsMessage(answer.Charts);
                }
            }
        }
        catch (Exception ex)
        {
            var text = "Sorry, I encountered an error processing your request: " + ex.Message;
            this.SendBotMessage(text);
            Console.WriteLine(ex);
        }
        finally
        {
            this.IsLoading = false;
        }
    }

    private void SendBotChartsMessage(List<ChartDataModel> charts)
    {
        this.ChatMessages.Add(new PickerItem
        {
            Context = new CardPickerContext
            {
                Cards = new List<CardContext>
                {
                    new ImageCardContext
                    {
                        Image = "chart.png",
                        Actions = new List<CardActionContext>
                        {
                            new CardActionContext
                            {
                                Text = "Open Chart",
                                Command = this.ToggleShouldShowChartsCommand,
                            }
                        }
                    }
                }
            }
        });

        this.SelectedCharts = charts.Take(3).Select(c => new TelerikChartModel(c)).ToList();
    }
}