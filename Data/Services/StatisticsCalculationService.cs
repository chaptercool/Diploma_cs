using Diploma_cs.Models;

namespace Diploma_cs.Data.Services;

public class StatisticsCalculationService
{
    private readonly AppDataService _appDataService;
    private DateTime? _lastWeeklyCalculationDate;

    public StatisticsCalculationService(AppDataService appDataService)
    {
        _appDataService = appDataService ?? throw new ArgumentNullException(nameof(appDataService));
    }

    public async Task InitializeAsync()
    {
        var latestWeeklyStats = await _appDataService.GetLatestWeeklyStatsAsync();
        if (latestWeeklyStats != null)
        {
            _lastWeeklyCalculationDate = latestWeeklyStats.EndDate;
        }
    }

    public async Task<bool> ShouldCalculateWeeklyStatsAsync()
    {
        var profile = await _appDataService.GetUserProfileAsync();
        if (profile == null)
            return false;

        var lastCalculation = _lastWeeklyCalculationDate ?? profile.SetupCompletedDate;
        var daysSinceLastCalculation = (DateTime.Now - lastCalculation).TotalDays;

        return daysSinceLastCalculation >= 7;
    }

    public async Task<bool> CalculateWeeklyStatsIfNeededAsync()
    {
        if (await ShouldCalculateWeeklyStatsAsync())
        {
            try
            {
                bool calculated = await _appDataService.CheckAndCalculateWeeklyStatsAsync();
                
                if (calculated)
                {
                    var latestWeeklyStats = await _appDataService.GetLatestWeeklyStatsAsync();
                    if (latestWeeklyStats != null)
                    {
                        _lastWeeklyCalculationDate = latestWeeklyStats.EndDate;
                    }
                }

                return calculated;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to calculate weekly stats", ex);
            }
        }

        return false;
    }

    public async Task<int> GetDaysUntilNextCalculationAsync()
    {
        var profile = await _appDataService.GetUserProfileAsync();
        if (profile == null)
            return 0;

        var lastCalculation = _lastWeeklyCalculationDate ?? profile.SetupCompletedDate;
        var daysSinceLastCalculation = (DateTime.Now - lastCalculation).TotalDays;
        var daysUntilNext = (int)Math.Ceiling(7 - daysSinceLastCalculation);

        return Math.Max(0, daysUntilNext);
    }

    public async Task<int> GetCalculationProgressPercentageAsync()
    {
        var profile = await _appDataService.GetUserProfileAsync();
        if (profile == null)
            return 0;

        var lastCalculation = _lastWeeklyCalculationDate ?? profile.SetupCompletedDate;
        var daysSinceLastCalculation = (DateTime.Now - lastCalculation).TotalDays;
        var progress = (int)((daysSinceLastCalculation / 7.0) * 100);

        return Math.Min(100, Math.Max(0, progress));
    }
}