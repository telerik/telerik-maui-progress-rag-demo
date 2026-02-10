using MauiProgressRagDemo.Services;
using MauiProgressRagDemo.Behaviors;
using System.Collections.ObjectModel;
using Telerik.Maui.Controls.Chat;

namespace MauiProgressRagDemo.ViewModels
{
    public abstract class ChatViewModelBase : ViewModelBase
    {
        private string chatTitle;
        private bool hasConversationStarted;
        private bool isTypingIndicatorVisible;
        protected bool isInitialMessage = true;
        private ChatScrollToBottomCommand scrollToBottomCommand;
        private string currentMessage = string.Empty;

        public ChatViewModelBase(NucliaSearchService nucliaSearchService) : base(nucliaSearchService)
        {
            this.ChatMessages = new ObservableCollection<ChatItem>();
            this.SuggestedChatMessages = new ObservableCollection<string>();
            this.TypingIndicatorAuthors = new ObservableCollection<Author>();
            this.UserAuthor = new Author() { Name = "User" };
            this.BotAuthor = new Author() { Avatar = "samplebot.png" };
            this.ScrollToBottomCommand = new ChatScrollToBottomCommand();
        }

        public string ChatTitle
        {
            get => this.chatTitle;
            set
            {
                this.chatTitle = value;
                this.OnPropertyChanged();
            }
        }

        public bool HasConversationStarted
        {
            get => this.hasConversationStarted;
            set
            {
                this.hasConversationStarted = value;
                this.OnPropertyChanged();
            }
        }

        public bool IsTypingIndicatorVisible
        {
            get => this.isTypingIndicatorVisible;
            set
            {
                this.isTypingIndicatorVisible = value;
                this.OnPropertyChanged();
            }
        }

        public ChatScrollToBottomCommand ScrollToBottomCommand
        {
            get => this.scrollToBottomCommand;
            set
            {
                this.scrollToBottomCommand = value;
                this.OnPropertyChanged();
            }
        }

        public string CurrentMessage
        {
            get => this.currentMessage;
            set
            {
                this.currentMessage = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<ChatItem> ChatMessages { get; set; }
        public Author BotAuthor { get; set; }
        public Author UserAuthor { get; set; }
        public Command SendChatMessageCommand => new Command(async (object param) => await SendMessageAsync(param));
        public ObservableCollection<Author> TypingIndicatorAuthors { get; set; }
        public ObservableCollection<string> SuggestedChatMessages { get; set; }

        protected abstract Task SendMessageAsync(object param);

        protected void ShowTypingIndicator(Author author)
        {
            this.TypingIndicatorAuthors.Add(author);
            this.IsTypingIndicatorVisible = true;
        }

        protected void HideTypingIndicator()
        {
            this.TypingIndicatorAuthors.Clear();
            this.IsTypingIndicatorVisible = false;
        }

        protected void SendBotMessage(string text)
        {
            var botMessage = new TextMessage
            {
                Author = this.BotAuthor,
                Text = text
            };

            this.ChatMessages.Add(botMessage);

            if (this.IsTypingIndicatorVisible)
            {
                this.HideTypingIndicator();
            }
        }

        public override void ResetState()
        {
            base.ResetState();
            
            this.ChatMessages.Clear();
            this.TypingIndicatorAuthors.Clear();
            this.ChatTitle = string.Empty;
            this.CurrentMessage = string.Empty;
            this.HasConversationStarted = false;
            this.IsTypingIndicatorVisible = false;
            this.isInitialMessage = true;
        }
    }
}
