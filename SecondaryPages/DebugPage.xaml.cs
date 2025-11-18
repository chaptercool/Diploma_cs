using System.Globalization;
using System.Runtime.CompilerServices;
using Microsoft.ML;

namespace Diploma_cs.SecondaryPages;

public partial class DebugPage : ContentPage
{
	public DebugPage()
	{
		InitializeComponent();
	}

	private async void OnPredictClicked(object sender, EventArgs e)
	{
        var avgConsumedText = AvgConsumed?.Text?.Trim() ?? string.Empty;
        var avgBoughtText = AvgBoughtPacks?.Text?.Trim() ?? string.Empty;
        var boughtSumText = BoughtSum?.Text?.Trim() ?? string.Empty;

        const NumberStyles style = NumberStyles.Float | NumberStyles.AllowThousands;
        var culture = CultureInfo.InvariantCulture;

        if (!float.TryParse(avgConsumedText, style, culture, out var avgConsumed))
            avgConsumed = 0f;

        if (!float.TryParse(avgBoughtText, style, culture, out var avgBought))
            avgBought = 0f;

        if (!float.TryParse(boughtSumText, style, culture, out var boughtSum))
            boughtSum = 0f;

        var sampleData = new MainComputingModule.ModelInput()
        {
            AvgConsumedWeek = avgConsumed,
            AvgBoughtPacks = avgBought,
            BoughtSum = boughtSum,
        };

        if (avgConsumed < 0 || avgBought < 0 || boughtSum < 0)
        {
            await DisplayAlert("Input Error", "Inconsistent values", "OK");
            return;
        }

        try
        {
            var result = MainComputingModule.Predict(sampleData);

            // Show both fields while you verify which one is the true prediction
            await DisplayAlert("Prediction Result",
                $"ReduceBy: {result.Score:F2}",
                "Close");
        }
        catch (FileNotFoundException fnf)
        {
            await DisplayAlert("Model file not found", fnf.Message, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Prediction error", ex.Message, "OK");
        }
    }

}