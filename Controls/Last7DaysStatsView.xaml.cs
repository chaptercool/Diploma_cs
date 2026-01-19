using Diploma_cs.Models;

namespace Diploma_cs.Controls;

public partial class Last7DaysStatsView : ContentView
{
    public static readonly BindableProperty StatsProperty = BindableProperty.Create(
        nameof(Stats),
        typeof(Last7DaysStatistics),
        typeof(Last7DaysStatsView),
        defaultValue: null,
        propertyChanged: (b, _, __) => ((Last7DaysStatsView)b).Apply());

    public Last7DaysStatistics? Stats
    {
        get => (Last7DaysStatistics?)GetValue(StatsProperty);
        set => SetValue(StatsProperty, value);
    }

    public Last7DaysStatsView()
    {
        InitializeComponent();
        Apply();
    }

    private void Apply()
    {
        var stats = Stats;
        if (stats == null || !stats.HasEnoughData())
        {
            FallbackBorder.IsVisible = true;
            ChartsContainer.IsVisible = false;
            return;
        }

        FallbackBorder.IsVisible = false;
        ChartsContainer.IsVisible = true;

        //ConsumptionChart.Title = "Konsumpcja";
        ConsumptionChart.Labels = stats.Labels;
        ConsumptionChart.Values = stats.DailyConsumption;

        //MoneyChart.Title = "Wydatki za ostatnie dni";
        MoneyChart.Labels = stats.Labels;
        MoneyChart.Values = stats.MoneySpent;

        ExceedBorder.IsVisible = stats.ExceededDaysCount > 0;

        if (stats.ExceededDaysCount > 0)
        {
            ExceedChart.Title = "Cel przekroczono";
            ExceedChart.Labels = stats.Labels;
            ExceedChart.Values = stats.TargetExceededBy;

            ExceedInfo.Text = $"Cel przekroczono {stats.ExceededDaysCount} dzieñ/dni w ci¹gu ostatnich 7 dni.";
        }
    }
}
