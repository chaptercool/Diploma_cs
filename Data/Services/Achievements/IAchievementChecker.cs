using Diploma_cs.Data.Services;

namespace Diploma_cs.Data.Services.Achievements;

public interface IAchievementChecker
{
    int AchievementID { get; }
    string CheckerName { get; }
    Task<bool> CheckAsync(AppDataService appDataService);
}