namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class NoSpendingDayChecker : IAchievementChecker
{
    public int AchievementID => 11;
    public string CheckerName => "No Spending Day";

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
            return dayDetail != null && dayDetail.PacksCount == 0;
        }
        catch
        {
            return false;
        }
    }
}