using MauiProgressRagDemo.Pages;

namespace MauiProgressRagDemo
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            
            Routing.RegisterRoute("Home", typeof(HomePage));
            Routing.RegisterRoute("IntelligentSearch", typeof(IntelligentSearchPage));
            Routing.RegisterRoute("FinancialAnalysis", typeof(FinancialAnalysisPage));
            Routing.RegisterRoute("KnowledgeAssistant", typeof(KnowledgeAssistantPage));
            Routing.RegisterRoute("AgenticRagValue", typeof(AgenticRagValuePage));
        }
    }
}
