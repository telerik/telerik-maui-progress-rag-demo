using MauiProgressRagDemo.Services;
using Telerik.Maui.Controls.Chat;

namespace MauiProgressRagDemo.ViewModels
{
    public class KnowledgeAssistantViewModel : ChatViewModelBase
    {
        public KnowledgeAssistantViewModel(NucliaSearchService nucliaSearchService) : base(nucliaSearchService)
        {
            this.SuggestedChatMessages = new()
            {
                "How do I get started with KendoReact components?",
                "What are the best KendoReact components for data visualization?",
                "How to implement theming and styling in KendoReact?"
            };
        }

        protected override async Task SendMessageAsync(object param)
        {
            var messageText = param as string ?? this.CurrentMessage;

            if (this.isInitialMessage)
            {
                var userMessage = new TextMessage
                {
                    Author = this.UserAuthor,
                    Text = messageText
                };

                this.ChatMessages.Add(userMessage);
                this.isInitialMessage = false;
                this.ChatTitle = messageText;
                this.HasConversationStarted = true;
            }

            this.IsLoading = true;
            this.ShowTypingIndicator(this.BotAuthor);

            var botMessage = new TextMessage
            {
                Author = this.BotAuthor,
                Text = string.Empty
            };

            try
            {
                await this.NucliaService.AskAsync(messageText, (partialResponse) =>
                {
                    if (!this.ChatMessages.Contains(botMessage))
                    {
                        this.ChatMessages.Add(botMessage);
                    }

                    botMessage.Text = partialResponse;
                    if (this.IsTypingIndicatorVisible)
                    {
                        this.HideTypingIndicator();
                    }

                    this.ScrollToBottomCommand.Execute(null);
                });
            }
            catch (Exception ex)
            {
                this.ChatMessages.Remove(botMessage);
                this.SendBotMessage("Sorry, I encountered an error processing your request: " + ex.Message);

                Console.WriteLine(ex);
            }
            finally
            {
                this.IsLoading = false;
                this.ScrollToBottomCommand.Execute(null);
            }
        }
    }
}
