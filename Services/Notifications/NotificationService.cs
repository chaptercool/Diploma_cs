using Diploma_cs.Models;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace Diploma_cs.Services.Notifications;

public sealed class NotificationService : IAppNotificationService
{
    private const int ReminderId1 = 10010;
    private const int ReminderId2 = 10011;
    private const int ReminderId3 = 10012;

    private const string AndroidChannelId = "daily_reminders";

    private readonly AppNotificationSettings _settings;

    public NotificationService(AppNotificationSettings settings)
    {
        _settings = settings;
    }

    public async Task<bool> EnsurePermissionsAsync()
    {
#if ANDROID
        if (Android.OS.Build.VERSION.SdkInt < Android.OS.BuildVersionCodes.Tiramisu)
            return true;
#endif

        var result = await LocalNotificationCenter.Current.RequestNotificationPermission();
        return result;
    }

    public async Task RescheduleDailyRemindersAsync()
    {
        if (!_settings.RemindersEnabled)
        {
            await CancelDailyRemindersAsync();
            return;
        }

        var granted = await EnsurePermissionsAsync();
        if (!granted)
            return;

        await CancelDailyRemindersAsync();

        await ScheduleDailyReminderAsync(ReminderId1, _settings.ReminderTime1);
        await ScheduleDailyReminderAsync(ReminderId2, _settings.ReminderTime2);
        await ScheduleDailyReminderAsync(ReminderId3, _settings.ReminderTime3);
    }

    public async Task CancelDailyRemindersAsync()
    {
        LocalNotificationCenter.Current.Cancel(ReminderId1);
        LocalNotificationCenter.Current.Cancel(ReminderId2);
        LocalNotificationCenter.Current.Cancel(ReminderId3);
        await Task.CompletedTask;
    }

    public async Task ShowAchievementUnlockedAsync(Achievement achievement)
    {
        var granted = await EnsurePermissionsAsync();
        if (!granted)
            return;

        var request = new NotificationRequest
        {
            NotificationId = CreateAchievementNotificationId(achievement.AchievementID),
            Title = "Nowe osi¹gniêcie zosta³o odblokowane!",
            Description = achievement.Name,
            Android = new AndroidOptions
            {
                ChannelId = AndroidChannelId,
                Priority = AndroidPriority.High
            }
        };

        await LocalNotificationCenter.Current.Show(request);
    }

    private static int CreateAchievementNotificationId(int achievementId)
        => 20000 + achievementId;

    private Task ScheduleDailyReminderAsync(int notificationId, TimeSpan time)
    {
        var now = DateTime.Now;
        var first = new DateTime(now.Year, now.Month, now.Day, time.Hours, time.Minutes, 0);
        if (first <= now)
            first = first.AddDays(1);

        var request = new NotificationRequest
        {
            NotificationId = notificationId,
            Title = "Przypomnienie",
            Description = "Nie zapomnij zarejestrowaæ aktywnoœci w aplikacji.",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = first,
                RepeatType = NotificationRepeat.Daily
            },
            Android = new AndroidOptions
            {
                ChannelId = AndroidChannelId,
                Priority = AndroidPriority.Default
            }
        };

        return LocalNotificationCenter.Current.Show(request);
    }
}
