namespace Diploma_cs.Data.Services.Achievements.Checkers;

public class GoodStartAchievementChecker : IAchievementChecker
{
    public int AchievementID => 1;
    public string CheckerName => "Good Start";

    public async Task<bool> CheckAsync(AppDataService appDataService)
    {
        try
        {
            var profile = await appDataService.GetUserProfileAsync();
            if (profile == null)
                return false;

            var hasName = !string.IsNullOrWhiteSpace(profile.Name);
            var hasAge = profile.Age > 0;
            var hasGender = !string.IsNullOrWhiteSpace(profile.Gender);
            var hasConsumption = profile.DailyConsumption > 0;

            System.Diagnostics.Debug.WriteLine($"GoodStartChecker: Name={hasName}, Age={hasAge}, Gender={hasGender}, Consumption={hasConsumption}");

            return hasName && hasAge && hasGender && hasConsumption;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GoodStartChecker error: {ex.Message}");
            return false;
        }
    }
}