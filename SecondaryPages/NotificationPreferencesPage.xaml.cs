using Diploma_cs.Services;
using Diploma_cs.Services.Notifications;

namespace Diploma_cs.SecondaryPages;

public partial class NotificationPreferencesPage : ContentPage
{
    private readonly AppNotificationSettings _settings;
    private readonly IAppNotificationService _notificationService;

    public NotificationPreferencesPage()
    {
        InitializeComponent();

        _settings = ServiceHelper.GetService<AppNotificationSettings>();
        _notificationService = ServiceHelper.GetService<IAppNotificationService>();

        RemindersEnabledSwitch.IsToggled = _settings.RemindersEnabled;
        Time1Picker.Time = _settings.ReminderTime1;
        Time2Picker.Time = _settings.ReminderTime2;
        Time3Picker.Time = _settings.ReminderTime3;

        RemindersEnabledSwitch.Toggled += (_, __) => UpdateEnabledState();
        UpdateEnabledState();
    }

    private void UpdateEnabledState()
    {
        var enabled = RemindersEnabledSwitch.IsToggled;
        Time1Picker.IsEnabled = enabled;
        Time2Picker.IsEnabled = enabled;
        Time3Picker.IsEnabled = enabled;
        SaveButton.IsEnabled = true;
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        _settings.RemindersEnabled = RemindersEnabledSwitch.IsToggled;
        _settings.ReminderTime1 = Time1Picker.Time;
        _settings.ReminderTime2 = Time2Picker.Time;
        _settings.ReminderTime3 = Time3Picker.Time;

        await _notificationService.RescheduleDailyRemindersAsync();

        await DisplayAlert("Zapisano", "Ustawienia powiadomieñ zosta³y zapisane.", "OK");
    }
}
