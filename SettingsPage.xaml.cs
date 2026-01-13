using Diploma_cs.Data.Services;
using Diploma_cs.SecondaryPages;

namespace Diploma_cs;

public partial class SettingsPage : ContentPage
{
    private int tapCount = 0;
    private DateTime lastTapTime = DateTime.MinValue;
    private const int TapsTarget = 5;
    private const int ResetCountdownMs = 1000;

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var profileService = new CsvUserProfileService();
            var profile = await profileService.GetUserProfileAsync();

            var name = profile?.Name?.Trim();

            GreetingLabel.Text = string.IsNullOrWhiteSpace(name)
                ? "Czeœæ!"
                : $"Czeœæ, {name}!";
        }
        catch
        {
            GreetingLabel.Text = "Czeœæ!";
        }
    }

    public async void OnLabelTapped(object sender, TappedEventArgs e)
    {
        if (DateTime.Now - lastTapTime > TimeSpan.FromMilliseconds(ResetCountdownMs))
        {
            tapCount = 0;
        }

        tapCount++;
        lastTapTime = DateTime.Now;

        if (tapCount == TapsTarget)
        {
            tapCount = 0;
            await Navigation.PushAsync(new DebugPage());
        }
    }

    public async void OnPrivacyTapped(object sender, EventArgs e) => await Navigation.PushAsync(new PrivacyStatementPage());

    public async void OnAboutTapped(object sender, EventArgs e) => await Navigation.PushAsync(new AboutAppPage());

    public async void OnSettingTapped(object sender, EventArgs e)
        => await Navigation.PushAsync(new PreferencesPage());
}