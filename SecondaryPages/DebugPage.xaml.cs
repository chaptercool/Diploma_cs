using System.Globalization;
using Diploma_cs.Data.Services;
using Diploma_cs.Services;
using Diploma_cs.Services.Notifications;
using Diploma_cs.Setup;

namespace Diploma_cs.SecondaryPages;

public partial class DebugPage : ContentPage
{
    private readonly AppDataService _appDataService;

    public DebugPage()
    {
        InitializeComponent();
        _appDataService = ServiceHelper.GetService<AppDataService>();

        // Sample data to demonstrate drawing with SkiaSharp.
        SampleChart.Labels = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
        SampleChart.Values = new float[] { 3, 6, 4, 7, 2, 5, 1 };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAndDisplayDataAsync();
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

    private async void OnSetupClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Warning", "Enter setup process? This may cause data corruption!", "Yes", "No");
        if (result)
        {
            Application.Current!.MainPage = new NavigationPage(new SetupStartPage());
        }
    }

    private async Task LoadAndDisplayDataAsync()
    {
        try
        {
            var sessionsCount = await _appDataService.GetTodaySessionsCountAsync();
            SessionsCountLabel.Text = sessionsCount.ToString();

            var packsCount = await _appDataService.GetTodayPacksCountAsync();
            PacksCountLabel.Text = packsCount.ToString();

            var userProfile = await _appDataService.GetUserProfileAsync();
            if (userProfile != null)
            {
                decimal amountSpent = (decimal)(packsCount * userProfile.PackPrice);
                AmountSpentLabel.Text = $"{amountSpent:F2} z³";
            }
            else
            {
                AmountSpentLabel.Text = "N/A";
            }

            var currentTarget = await _appDataService.GetCurrentTargetAsync();
            CurrentTargetLabel.Text = (currentTarget ?? 0).ToString();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load data: {ex.Message}", "OK");
        }
    }

    private async void OnSendTestNotificationClicked(object sender, EventArgs e)
    {
        try
        {
            var notificationService = ServiceHelper.GetService<IAppNotificationService>();
            var granted = await notificationService.EnsurePermissionsAsync();
            if (!granted)
            {
                await DisplayAlert("Notifications", "Notification permission not granted.", "OK");
                return;
            }

            await notificationService.CancelDailyRemindersAsync();
            await notificationService.RescheduleDailyRemindersAsync();

            await notificationService.ShowAchievementUnlockedAsync(new Models.Achievement
            {
                AchievementID = 9999,
                Name = "Test notification",
                DescriptionUnlocked = "This is a test notification from DebugPage."
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Notifications", $"Failed to send test notification: {ex.Message}", "OK");
        }
    }
}