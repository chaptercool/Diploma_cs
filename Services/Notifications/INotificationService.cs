using Diploma_cs.Models;

namespace Diploma_cs.Services.Notifications;

public interface IAppNotificationService
{
    Task<bool> EnsurePermissionsAsync();

    Task RescheduleDailyRemindersAsync();

    Task CancelDailyRemindersAsync();

    Task ShowAchievementUnlockedAsync(Achievement achievement);
}
