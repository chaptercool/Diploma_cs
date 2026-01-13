namespace Diploma_cs.Services.Notifications;

public sealed class AppNotificationSettings
{
    private const string RemindersEnabledKey = "notifications.reminders.enabled";
    private const string ReminderTime1Key = "notifications.reminders.time1";
    private const string ReminderTime2Key = "notifications.reminders.time2";
    private const string ReminderTime3Key = "notifications.reminders.time3";

    public bool RemindersEnabled
    {
        get => Preferences.Default.Get(RemindersEnabledKey, true);
        set => Preferences.Default.Set(RemindersEnabledKey, value);
    }

    public TimeSpan ReminderTime1
    {
        get => GetTimeSpan(ReminderTime1Key, new TimeSpan(10, 0, 0));
        set => SetTimeSpan(ReminderTime1Key, value);
    }

    public TimeSpan ReminderTime2
    {
        get => GetTimeSpan(ReminderTime2Key, new TimeSpan(14, 0, 0));
        set => SetTimeSpan(ReminderTime2Key, value);
    }

    public TimeSpan ReminderTime3
    {
        get => GetTimeSpan(ReminderTime3Key, new TimeSpan(19, 0, 0));
        set => SetTimeSpan(ReminderTime3Key, value);
    }

    private static TimeSpan GetTimeSpan(string key, TimeSpan defaultValue)
    {
        var value = Preferences.Default.Get(key, defaultValue.ToString("c"));
        return TimeSpan.TryParse(value, out var ts) ? ts : defaultValue;
    }

    private static void SetTimeSpan(string key, TimeSpan value)
        => Preferences.Default.Set(key, value.ToString("c"));
}
