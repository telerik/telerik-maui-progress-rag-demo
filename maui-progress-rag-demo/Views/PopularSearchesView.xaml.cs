using System.Collections;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

namespace MauiProgressRagDemo.Views;

public partial class PopularSearchesView : ContentView
{
    public PopularSearchesView()
    {
        InitializeComponent();
    }

    public static readonly BindableProperty ItemsSourceProperty =
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(PopularSearchesView), null);

    public IEnumerable ItemsSource
    {
        get => (IEnumerable)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    public static readonly BindableProperty CommandProperty =
        BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(PopularSearchesView), null);

    public ICommand Command
    {
        get => (ICommand)GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly BindableProperty ButtonStyleProperty =
        BindableProperty.Create(nameof(ButtonStyle), typeof(Style), typeof(PopularSearchesView), null);

    public Style ButtonStyle
    {
        get => (Style)GetValue(ButtonStyleProperty);
        set => SetValue(ButtonStyleProperty, value);
    }

    public static readonly BindableProperty OrientationProperty =
        BindableProperty.Create(nameof(Orientation), typeof(Microsoft.Maui.Controls.StackOrientation), typeof(PopularSearchesView), Microsoft.Maui.Controls.StackOrientation.Horizontal);

    public Microsoft.Maui.Controls.StackOrientation Orientation
    {
        get => (Microsoft.Maui.Controls.StackOrientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    // Expose simple layout properties so page can customize without wrapping
    public new static readonly BindableProperty HorizontalOptionsProperty =
        BindableProperty.Create(nameof(HorizontalOptions), typeof(LayoutOptions), typeof(PopularSearchesView), LayoutOptions.Start);

    public new LayoutOptions HorizontalOptions
    {
        get => (LayoutOptions)GetValue(HorizontalOptionsProperty);
        set => SetValue(HorizontalOptionsProperty, value);
    }

    public new static readonly BindableProperty VerticalOptionsProperty =
        BindableProperty.Create(nameof(VerticalOptions), typeof(LayoutOptions), typeof(PopularSearchesView), LayoutOptions.Center);

    public new LayoutOptions VerticalOptions
    {
        get => (LayoutOptions)GetValue(VerticalOptionsProperty);
        set => SetValue(VerticalOptionsProperty, value);
    }

    // Padding property re-used as Margin binding target above
    public static readonly BindableProperty PaddingProperty =
        BindableProperty.Create(nameof(Padding), typeof(Thickness), typeof(PopularSearchesView), new Thickness(0));

    public Thickness Padding
    {
        get => (Thickness)GetValue(PaddingProperty);
        set => SetValue(PaddingProperty, value);
    }
}