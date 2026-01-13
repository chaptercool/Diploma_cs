using System.Diagnostics;
using Diploma_cs.Data.Services.Achievements.Checkers;
using Diploma_cs.Models;
using Diploma_cs.Services;
using Diploma_cs.Services.Notifications;

namespace Diploma_cs.Data.Services.Achievements;

public class AchievementService
{
    private readonly AppDataService _appDataService;
    private readonly IAchievementChecker[] _checkers;
    private readonly AchievementRepository _achievementRepository;

    public AchievementService(AppDataService appDataService)
    {
        _appDataService = appDataService;
        _achievementRepository = new AchievementRepository();

        _checkers = new IAchievementChecker[]
        {
            new GoodStartAchievementChecker(),
            new FirstChangesAchievementChecker(),
            new SavingsMasterAchievementChecker(),
            new StreakAchievementChecker(4, 7),
            new StreakAchievementChecker(5, 14),
            new StreakAchievementChecker(6, 21),
            new StreakAchievementChecker(7, 28),
            new BetterChangesAchievementChecker(),
            new CleanDayAchievementChecker(),
            new ConsistentRegistrationChecker(),
            new NoSpendingDayChecker(),
            new FirstExceedAchievementChecker()
        };
    }

    public async Task<List<Achievement>> CheckAllAchievementsAsync()
    {
        var newlyUnlocked = new List<Achievement>();

        foreach (var checker in _checkers)
        {
            try
            {
                Debug.WriteLine($"AchievementService: Checking '{checker.CheckerName}' (#{checker.AchievementID})...");

                bool isMet = await checker.CheckAsync(_appDataService);

                Debug.WriteLine($"AchievementService: Checker '{checker.CheckerName}' returned {isMet}");

                if (isMet && !await _achievementRepository.IsAchievementUnlockedAsync(checker.AchievementID))
                {
                    var achievement = await _achievementRepository.GetAchievementByIdAsync(checker.AchievementID);
                    if (achievement != null)
                    {
                        achievement.IsUnlocked = true;
                        achievement.UnlockedDate = DateTime.Now;

                        await _achievementRepository.SaveUnlockedAchievementAsync(checker.AchievementID, DateTime.Now);
                        newlyUnlocked.Add(achievement);

                        try
                        {
                            var notificationService = ServiceHelper.TryGetService<IAppNotificationService>();
                            if (notificationService != null)
                                await notificationService.ShowAchievementUnlockedAsync(achievement);
                        }
                        catch (Exception notifyEx)
                        {
                            Debug.WriteLine($"AchievementService: Failed to send notification: {notifyEx.Message}");
                        }

                        Debug.WriteLine($"Achievement unlocked: {checker.CheckerName} (#{checker.AchievementID})");
                    }
                    else
                    {
                        Debug.WriteLine($"AchievementService: Could not find achievement metadata for ID {checker.AchievementID}");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking achievement {checker.CheckerName}: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        return newlyUnlocked;
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        return await _achievementRepository.GetAllAchievementsAsync();
    }

    public async Task<List<Achievement>> GetUnlockedAchievementsAsync()
    {
        var all = await _achievementRepository.GetAllAchievementsAsync();
        return all.Where(a => a.IsUnlocked).ToList();
    }

    public async Task<int> GetUnlockedCountAsync()
    {
        var unlocked = await GetUnlockedAchievementsAsync();
        return unlocked.Count;
    }

    public async Task<int> GetTotalCountAsync()
    {
        var all = await _achievementRepository.GetAllAchievementsAsync();
        return all.Count;
    }
}