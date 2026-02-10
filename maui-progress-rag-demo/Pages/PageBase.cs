namespace MauiProgressRagDemo.Pages;

using MauiProgressRagDemo.ViewModels;

public class PageBase : ContentPage
{
    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (this.BindingContext is ViewModelBase viewModel)
        {
            viewModel.ResetState();
        }
    }
}
