using MauiProgressRagDemo.Services;

namespace MauiProgressRagDemo.Models;

public class TelerikChartModel
{
    public TelerikChartModel(ChartDataModel nucliaChartModel)
    {
        this.Title = nucliaChartModel.Title;
        this.Series = new List<TelerikChartSeriesItem>();
        this.BuildSeries(nucliaChartModel);
    }

    public string Title { get; private set; }
    public List<TelerikChartSeriesItem> Series { get; private set; }

    private void BuildSeries(ChartDataModel nucliaChartModel)
    {
        for (int i = 0; nucliaChartModel.Series.Count > i; i++)
        {
            var current = nucliaChartModel.Series[i];
            var seriesItem = new TelerikChartSeriesItem
            {
                Name = current.Name,
                Data = new List<TelerikChartDataItem>()
            };

            var dataCount = Math.Min(current.Data.Count, 3);
            for (int j = 0; dataCount > j; j++)
            {
                var dataItem = new TelerikChartDataItem
                {
                    Category = nucliaChartModel.Categories[j],
                    Value = current.Data[j],
                };

                seriesItem.Data.Add(dataItem);
            }

            this.Series.Add(seriesItem);
        }
    }
}

public class TelerikChartSeriesItem
{         
    public string Name { get; set; }
    public List<TelerikChartDataItem> Data { get; set; }
}

public class TelerikChartDataItem
{
    public string Category { get; set; }
    public double Value { get; set; }
}
