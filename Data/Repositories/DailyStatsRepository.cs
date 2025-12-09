using Diploma_cs.Data.Services;
using Diploma_cs.Models;

namespace Diploma_cs.Data.Repositories;

public class DailyStatsRepository
{
    private readonly CsvDailyStatsService _dailyStatsService;
    private readonly SessionRepository _sessionRepository;
    private readonly TargetRepository _targetRepository;

    public DailyStatsRepository(CsvDailyStatsService dailyStatsService, 
                              SessionRepository? sessionRepository = null,
                              TargetRepository? targetRepository = null)
    {
        _dailyStatsService = dailyStatsService ?? throw new ArgumentNullException(nameof(dailyStatsService));
        _sessionRepository = sessionRepository;
        _targetRepository = targetRepository;
    }

    public async Task UpdateDailyStatsAsync(DateTime date)
    {
        if (_sessionRepository == null)
            throw new InvalidOperationException("SessionRepository is required for daily stats update");

        var sessionsCount = await _sessionRepository.GetSmokingSessionsCountAsync(date);
        var packsCount = await _sessionRepository.GetPackPurchasesCountAsync(date);

        int dailyTarget = 0;
        if (_targetRepository != null)
        {
            var targetRecord = await _targetRepository.GetTargetForDateAsync(date);
            dailyTarget = targetRecord ?? 0;
        }

        var stats = new DailyStats(date, sessionsCount, packsCount, dailyTarget);
        await _dailyStatsService.SaveDailyStatsAsync(stats);
    }

    public async Task<DailyStats?> GetDailyStatsByDateAsync(DateTime date)
    {
        return await _dailyStatsService.GetDailyStatsByDateAsync(date);
    }

    public async Task<List<DailyStats>> GetAllDailyStatsAsync()
    {
        return await _dailyStatsService.GetAllDailyStatsAsync();
    }

    public async Task<List<DailyStats>> GetDailyStatsInRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dailyStatsService.GetDailyStatsInRangeAsync(startDate, endDate);
    }

    public async Task<DailyStats?> GetLatestDailyStatsAsync()
    {
        return await _dailyStatsService.GetLatestDailyStatsAsync();
    }

    public async Task<DailyStats.DayStatus?> GetDayStatusAsync(DateTime date)
    {
        var stats = await GetDailyStatsByDateAsync(date);
        return stats?.GetDayStatus();
    }

    public async Task ClearAllDailyStatsAsync()
    {
        await _dailyStatsService.ClearAllDailyStatsAsync();
    }

    public string GetDailyStatsFilePath() => _dailyStatsService.GetDailyStatsFilePath();
}