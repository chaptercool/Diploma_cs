namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class SavingsMasterAchievementChecker : IAchievementChecker
{
    public int AchievementID => 3;
    public string CheckerName => "Savings Master";

    public async Task<bool> CheckAsync(AppDataService appDataService)
    {
        try
        {
            var profile = await appDataService.GetUserProfileAsync();
            if (profile == null) return false;

            int totalPacksBought = await GetTotalPacksBoughtAsync(appDataService);
            decimal totalSaved = (decimal)(totalPacksBought * profile.PackPrice);

            return totalSaved >= 100;
        }
        catch
        {
            return false;
        }
    }

    private async Task<int> GetTotalPacksBoughtAsync(AppDataService appDataService)
    {
        try
        {
            // This will be implemented as an extension to AppDataService
            // For now, we'll use a temporary solution by checking daily stats
            var today = DateTime.Now;
            var firstDay = today.AddYears(-1); // Check past year
            var allDailyStats = await appDataService.GetDailyStatsRangeAsync(firstDay, today);
            
            int totalPacks = 0;
            foreach (var stat in allDailyStats)
            {
                var dayDetail = await appDataService.GetDayDetailAsync(stat.Date);
                totalPacks += dayDetail.PacksCount;
            }

            return totalPacks;
        }
        catch
        {
            return 0;
        }
    }
}