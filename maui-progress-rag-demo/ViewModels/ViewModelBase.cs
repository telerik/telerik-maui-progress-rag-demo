using System.Windows.Input;
using MauiProgressRagDemo.Services;
using Telerik.Maui.Controls;

namespace MauiProgressRagDemo.ViewModels;

public class ViewModelBase : NotifyPropertyChangedBase
{
    private bool isLoading;

    public ViewModelBase(NucliaSearchService nucliaSearchService)
    {
        this.NucliaService = nucliaSearchService;
        this.NavigateCommand = new Command<string>(async pageRoute => await NavigateAsync(pageRoute));
    }

    public bool IsLoading
    {
        get => this.isLoading;
        set
        {
            if (this.isLoading != value)
            {
                this.isLoading = value;
                this.OnPropertyChanged();
            }
        }
    }

    protected NucliaSearchService NucliaService { get; }
    public ICommand NavigateCommand { get; }

    /// <summary>
    /// Called when the page appears. Override to reset ViewModel state.
    /// </summary>
    public virtual void ResetState()
    {
        this.IsLoading = false;
    }

    private static async Task NavigateAsync(string pageRoute)
    {
        if (string.IsNullOrWhiteSpace(pageRoute) || Shell.Current == null)
        {
            return;
        }

        var target = pageRoute.StartsWith("///", StringComparison.Ordinal)
            ? pageRoute
            : $"///{pageRoute.TrimStart('/')}";

        await Shell.Current.GoToAsync(target);
    }
}