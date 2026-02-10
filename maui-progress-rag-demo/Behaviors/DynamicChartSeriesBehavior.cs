using MauiProgressRagDemo.Models;
using Telerik.Maui.Controls.Compatibility.Chart;

namespace MauiProgressRagDemo.Behaviors;

public class DynamicChartSeriesBehavior : Behavior<RadCartesianChart>
{
    private RadCartesianChart? associatedChart;

    protected override void OnAttachedTo(RadCartesianChart chart)
    {
        base.OnAttachedTo(chart);
        this.associatedChart = chart;
        chart.BindingContextChanged += this.OnBindingContextChanged;
        
        // Initial update if binding context already set
        if (chart.BindingContext != null)
        {
            this.UpdateSeries();
        }
    }

    protected override void OnDetachingFrom(RadCartesianChart chart)
    {
        base.OnDetachingFrom(chart);
        chart.BindingContextChanged -= this.OnBindingContextChanged;
        this.associatedChart = null;
    }

    private void OnBindingContextChanged(object? sender, EventArgs e)
    {
        this.UpdateSeries();
    }

    private void UpdateSeries()
    {
        if (this.associatedChart?.BindingContext is not TelerikChartModel model)
        {
            return;
        }

        // Clear existing series
        this.associatedChart.Series.Clear();

        // Create a BarSeries for each series in the model
        foreach (var seriesItem in model.Series)
        {
            var barSeries = new BarSeries
            {
                ItemsSource = seriesItem.Data,
                CategoryBinding = new PropertyNameDataPointBinding("Category"),
                ValueBinding = new PropertyNameDataPointBinding("Value"),
                CombineMode = ChartSeriesCombineMode.Cluster,
                DisplayName = seriesItem.Name
            };

            this.associatedChart.Series.Add(barSeries);
        }
    }
}
