namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class FirstExceedAchievementChecker : IAchievementChecker
{
    public int AchievementID => 12;
    public string CheckerName => "First Exceed";

    public async Task<bool> CheckAsync(AppDataService appDataService)
    {
        try
        {
            var allDailyStats = await appDataService.GetAllWeeklyStatsAsync();
            if (allDailyStats.Count == 0)
                return false;

            var today = DateTime.Now;
            var firstDay = today.AddMonths(-1);
            var monthStats = await appDataService.GetDailyStatsRangeAsync(firstDay, today);

            foreach (var stat in monthStats)
            {
                if (stat.GetDayStatus() == Models.DailyStats.DayStatus.Exceeded)
                    return true;
            }

            return false;
        }
        catch
        {
            return false;
        }
    }
}