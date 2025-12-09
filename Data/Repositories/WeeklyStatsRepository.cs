using Diploma_cs.Data.Services;
using Diploma_cs.Models;

namespace Diploma_cs.Data.Repositories;

public class WeeklyStatsRepository
{
    private readonly CsvWeeklyStatsService _weeklyStatsService;
    private readonly DailyStatsRepository _dailyStatsRepository;

    public WeeklyStatsRepository(CsvWeeklyStatsService weeklyStatsService, 
                               DailyStatsRepository? dailyStatsRepository = null)
    {
        _weeklyStatsService = weeklyStatsService ?? throw new ArgumentNullException(nameof(weeklyStatsService));
        _dailyStatsRepository = dailyStatsRepository;
    }

    public async Task<WeeklyStats?> CalculateAndSaveWeeklyStatsAsync()
    {
        if (_dailyStatsRepository == null)
            throw new InvalidOperationException("DailyStatsRepository is required for weekly stats calculation");

        var endDate = DateTime.Now.Date;
        var startDate = endDate.AddDays(-6);

        return await CalculateAndSaveWeeklyStatsAsync(startDate, endDate);
    }

    public async Task<WeeklyStats?> CalculateAndSaveWeeklyStatsAsync(DateTime startDate, DateTime endDate)
    {
        if (_dailyStatsRepository == null)
            throw new InvalidOperationException("DailyStatsRepository is required for weekly stats calculation");

        var dailyStats = await _dailyStatsRepository.GetDailyStatsInRangeAsync(startDate, endDate);

        if (dailyStats.Count == 0)
            return null;

        int totalSessions = dailyStats.Sum(d => d.SessionsCount);
        int totalPacks = dailyStats.Sum(d => d.PacksCount);

        float avgConsumedWeek = dailyStats.Count > 0 ? totalSessions / 7f : 0f;
        float avgBoughtPacks = dailyStats.Count > 0 ? totalPacks / 7f : 0f;
        float boughtSum = totalPacks;

        var weeklyStats = new WeeklyStats(startDate, endDate, avgConsumedWeek, avgBoughtPacks, boughtSum);
        await _weeklyStatsService.AddWeeklyStatsAsync(weeklyStats);

        return weeklyStats;
    }

    public async Task<WeeklyStats?> GetLatestWeeklyStatsAsync()
    {
        return await _weeklyStatsService.GetLatestWeeklyStatsAsync();
    }

    public async Task<List<WeeklyStats>> GetAllWeeklyStatsAsync()
    {
        return await _weeklyStatsService.GetAllWeeklyStatsAsync();
    }

    public async Task<List<WeeklyStats>> GetWeeklyStatsInRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _weeklyStatsService.GetWeeklyStatsInRangeAsync(startDate, endDate);
    }

    public async Task ClearAllWeeklyStatsAsync()
    {
        await _weeklyStatsService.ClearAllWeeklyStatsAsync();
    }

    public string GetWeeklyStatsFilePath() => _weeklyStatsService.GetWeeklyStatsFilePath();
}