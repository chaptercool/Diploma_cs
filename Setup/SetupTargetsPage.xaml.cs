using Diploma_cs.Models;
using Diploma_cs.Services;

namespace Diploma_cs.Setup;

public class SetupSessionData
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    public string Gender { get; set; } = string.Empty;
    public int DailyConsumption { get; set; }
    public float PackPrice { get; set; }
}

public partial class SetupTargetsPage : ContentPage
{
    public SetupSessionData? SetupSession { get; set; }

    public SetupTargetsPage()
    {
        InitializeComponent();
    }

    private async void OnNextButtonClicked(object sender, EventArgs e)
    {
        string DailyConsume = UserDailyConsume?.Text?.Trim() ?? string.Empty;
        string PackPrice = UserPackPrice?.Text?.Trim() ?? string.Empty;
        int dailyConsumeVal;
        float packPriceVal;

        if (string.IsNullOrWhiteSpace(DailyConsume) || string.IsNullOrWhiteSpace(PackPrice))
        {
            await DisplayAlert("Brak danych", "Proszê wype³niæ wszystkie pola.", "OK");
            return;
        }

        if (!int.TryParse(DailyConsume, out dailyConsumeVal))
        {
            await DisplayAlert("B³¹d", "Liczba dziennej konsumpcji musi byæ ca³kowita.", "OK");
            return;
        }

        if (!float.TryParse(PackPrice, out packPriceVal))
        {
            await DisplayAlert("B³¹d", "Cena paczki musi byæ liczb¹.", "OK");
            return;
        }

        if (dailyConsumeVal <= 0 || packPriceVal <= 0)
        {
            await DisplayAlert("B³¹d", "Wartoœci musz¹ byæ wiêksze od zera.", "OK");
            return;
        }

        try
        {
            if (SetupSession != null)
            {
                SetupSession.DailyConsumption = dailyConsumeVal;
                SetupSession.PackPrice = packPriceVal;
            }

            await Navigation.PushAsync(new SetupSummaryPage { SetupSession = SetupSession });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unknown error: {ex.Message}", "OK");
        }
    }
}