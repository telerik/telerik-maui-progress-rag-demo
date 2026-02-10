using MauiProgressRagDemo.Services;
using MauiProgressRagDemo.ViewModels;
using MauiProgressRagDemo.Pages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Logging;
using Progress.Nuclia;
using System.Reflection;
using Telerik.Maui.Controls.Compatibility;

namespace MauiProgressRagDemo
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseTelerik()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Metric-Regular.ttf", "MetricRegular");
                    fonts.AddFont("Metric-Medium.ttf", "MetricMedium");
                });

            AddConfig(builder);
            ConfigureNuclia(builder);
            ConfigureViewModelsAndPages(builder);

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }

        private static void AddConfig(MauiAppBuilder builder)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("MauiProgressRagDemo.appsettings.json");
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config); 
        }

        private static void ConfigureNuclia(MauiAppBuilder builder)
        {
            // Configure NucliaDB client
            var nucliaConfig = builder.Configuration.GetSection("NucliaDb");
            var config = new NucliaDbConfig(
                nucliaConfig["ZoneId"] ?? throw new InvalidOperationException("NucliaDb ZoneId not configured"),
                nucliaConfig["KnowledgeBoxId"] ?? throw new InvalidOperationException("NucliaDb KnowledgeBoxId not configured"),
                nucliaConfig["ApiKey"] ?? throw new InvalidOperationException("NucliaDb ApiKey not configured")
            );

            var nucliaChartsConfig = builder.Configuration.GetSection("NucliaDbCharts");
            var chartsConfig = new NucliaDbConfig(
                nucliaChartsConfig["ZoneId"] ?? throw new InvalidOperationException("NucliaDbCharts ZoneId not configured"),
                nucliaChartsConfig["KnowledgeBoxId"] ?? throw new InvalidOperationException("NucliaDbCharts KnowledgeBoxId not configured"),
                nucliaChartsConfig["ApiKey"] ?? throw new InvalidOperationException("NucliaDbCharts ApiKey not configured")
            );

            var nucliaVerseConfig = builder.Configuration.GetSection("NucliaDbVerse");
            var verseConfig = new NucliaDbConfig(
                nucliaVerseConfig["ZoneId"] ?? throw new InvalidOperationException("NucliaDbVerse ZoneId not configured"),
                nucliaVerseConfig["KnowledgeBoxId"] ?? throw new InvalidOperationException("NucliaDbVerse KnowledgeBoxId not configured"),
                nucliaVerseConfig["ApiKey"] ?? throw new InvalidOperationException("NucliaDbVerse ApiKey not configured")
            );

            builder.Services.AddScoped(sp =>
            {
                var defaultClient = new NucliaDbClient(config);
                var chartsClient = new NucliaDbClient(chartsConfig);
                var verseClient = new NucliaDbClient(verseConfig);
                return new NucliaSearchService(defaultClient, chartsClient, verseClient);
            });
        }

        private static void ConfigureViewModelsAndPages(MauiAppBuilder builder)
        {
            // Register Services
            builder.Services.AddSingleton<IToastMessageService, ToastMessageService>();
            
            // Register ViewModels as Singleton - state is reset via ResetState() on page appearing
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<IntelligentSearchViewModel>();
            builder.Services.AddSingleton<FinancialAnalysisViewModel>();
            builder.Services.AddSingleton<AgenticRagValueViewModel>();
            builder.Services.AddSingleton<KnowledgeAssistantViewModel>();
            
            // Register Pages as Singleton - cached by Shell, ViewModel state reset on appearing
            builder.Services.AddSingleton<HomePage>();
            builder.Services.AddSingleton<IntelligentSearchPage>();
            builder.Services.AddSingleton<FinancialAnalysisPage>();
            builder.Services.AddSingleton<AgenticRagValuePage>();
            builder.Services.AddSingleton<KnowledgeAssistantPage>();
        }
    }
}
