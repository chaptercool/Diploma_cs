namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class StreakAchievementChecker : IAchievementChecker
{
    private readonly int _streakDays;

    public int AchievementID { get; private set; }
    public string CheckerName => $"{_streakDays}-Day Streak";

    public StreakAchievementChecker(int achievementID, int streakDays)
    {
        AchievementID = achievementID;
        _streakDays = streakDays;
    }

    public async Task<bool> CheckAsync(AppDataService appDataService)
    {
        try
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-_streakDays + 1);

            var dailyStats = await appDataService.GetDailyStatsRangeAsync(startDate, endDate);

            if (dailyStats.Count < _streakDays)
                return false;

            foreach (var stat in dailyStats)
            {
                var status = stat.GetDayStatus();
                if (status == Models.DailyStats.DayStatus.Exceeded)
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}