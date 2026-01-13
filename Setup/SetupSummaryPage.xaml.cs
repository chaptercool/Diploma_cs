using Diploma_cs.Data.Services;
using Diploma_cs.Services;

namespace Diploma_cs.Setup;

public partial class SetupSummaryPage : ContentPage
{
    private readonly AppDataService _appDataService;
    public SetupSessionData? SetupSession { get; set; }

    public SetupSummaryPage()
    {
        InitializeComponent();
        _appDataService = ServiceHelper.GetService<AppDataService>();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        DisplaySetupSummary();
    }

    private void DisplaySetupSummary()
    {
        if (SetupSession == null)
            return;

        int initialTarget = SetupSession.DailyConsumption + 2;
        if (MainCounterBlock?.Children.FirstOrDefault() is Label targetLabel)
        {
            targetLabel.Text = initialTarget.ToString();
        }
    }

    private async void OnNextButtonClicked(object sender, EventArgs e)
    {
        if (SetupSession == null)
        {
            await DisplayAlert("B³¹d", "Brak danych o konfiguracji", "OK");
            return;
        }

        try
        {
            await _appDataService.CompleteSetupAsync(
                SetupSession.Name,
                SetupSession.Age,
                SetupSession.Gender,
                SetupSession.DailyConsumption,
                SetupSession.PackPrice
            );

            await Navigation.PushAsync(new SetupSuccessPage());
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê utworzyæ profil u¿ytkownika: {ex.Message}", "OK");
        }
    }
}