using MauiProgressRagDemo.ViewModels;

namespace MauiProgressRagDemo.Pages;

public partial class FinancialAnalysisPage : PageBase
{
    public FinancialAnalysisPage(FinancialAnalysisViewModel viewModel)
    {
        this.InitializeComponent();
        this.BindingContext = viewModel;
    }

#if WINDOWS || MACCATALYST
    private void RadPopup_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Telerik.Maui.Controls.RadPopup.PlacementTarget))
        {
            var popup = sender as Telerik.Maui.Controls.RadPopup;
            // Ensure the PlacementTarget is set to this page to center the popup correctly
            popup.PlacementTarget = this; 
            popup.PropertyChanged -= this.RadPopup_PropertyChanged; // Unsubscribe after setting the PlacementTarget
        }
    }
#endif
}