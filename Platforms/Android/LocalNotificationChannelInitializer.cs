#if ANDROID
using Android.App;
using Android.Content;
using Android.OS;

namespace Diploma_cs.Platforms.Android;

public static class LocalNotificationChannelInitializer
{
    public const string DailyRemindersChannelId = "daily_reminders";

    public static void EnsureChannels(Context context)
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            return;

        var manager = (NotificationManager?)context.GetSystemService(Context.NotificationService);
        if (manager == null)
            return;

        var existing = manager.GetNotificationChannel(DailyRemindersChannelId);
        if (existing != null)
            return;

        var channel = new NotificationChannel(
            DailyRemindersChannelId,
            "Reminders",
            NotificationImportance.Default)
        {
            Description = "Daily reminders and achievement notifications"
        };

        manager.CreateNotificationChannel(channel);
    }
}
#endif
