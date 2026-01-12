namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class ConsistentRegistrationChecker : IAchievementChecker
{
    public int AchievementID => 10;
    public string CheckerName => "Consistent Registration";

    public async Task<bool> CheckAsync(AppDataService appDataService)
    {
        try
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddDays(-6);

            var dailyStats = await appDataService.GetDailyStatsRangeAsync(startDate, endDate);

            if (dailyStats.Count < 7)
                return false;

            foreach (var stat in dailyStats)
            {
                var dayDetail = await appDataService.GetDayDetailAsync(stat.Date);
                if (dayDetail.SessionsCount == 0)
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