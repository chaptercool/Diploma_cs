namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class CleanDayAchievementChecker : IAchievementChecker
{
    public int AchievementID => 9;
    public string CheckerName => "Clean Day";

    public async Task<bool> CheckAsync(AppDataService appDataService)
    {
        try
        {
            var profile = await appDataService.GetUserProfileAsync();
            if (profile == null)
                return false;

            var daysSinceSetup = (DateTime.Now - profile.SetupCompletedDate).TotalDays;
            if (daysSinceSetup < 5)
                return false;

            var yesterday = DateTime.Now.AddDays(-1);
            var dayDetail = await appDataService.GetDayDetailAsync(yesterday);
            return dayDetail != null && dayDetail.SessionsCount == 0;
        }
        catch
        {
            return false;
        }
    }
}