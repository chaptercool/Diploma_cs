namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class BetterChangesAchievementChecker : IAchievementChecker
{
    public int AchievementID => 8;
    public string CheckerName => "Better Changes";

    public async Task<bool> CheckAsync(AppDataService appDataService)
    {
        try
        {
            var currentTarget = await appDataService.GetCurrentTargetAsync();
            return currentTarget == 10;
        }
        catch
        {
            return false;
        }
    }
}