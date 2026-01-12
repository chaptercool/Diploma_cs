namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class FirstChangesAchievementChecker : IAchievementChecker
{
    public int AchievementID => 2;
    public string CheckerName => "First Changes";

    public async Task<bool> CheckAsync(AppDataService appDataService)
    {
        try
        {
            var allWeeklyStats = await appDataService.GetAllWeeklyStatsAsync();
            return allWeeklyStats.Count > 1;
        }
        catch
        {
            return false;
        }
    }
}