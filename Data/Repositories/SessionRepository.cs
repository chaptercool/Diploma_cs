using Diploma_cs.Data.Services;
using Diploma_cs.Models;

namespace Diploma_cs.Data.Repositories;

public class SessionRepository
{
    private readonly CsvSessionService _sessionService;
    private DailyStatsRepository? _dailyStatsRepository;

    public SessionRepository(CsvSessionService sessionService, DailyStatsRepository? dailyStatsRepository = null)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _dailyStatsRepository = dailyStatsRepository;
    }

    public void SetDailyStatsRepository(DailyStatsRepository dailyStatsRepository)
    {
        _dailyStatsRepository = dailyStatsRepository ?? throw new ArgumentNullException(nameof(dailyStatsRepository));
    }

    public async Task RegisterSessionAsync(string sessionType = "Smoking")
    {
        var session = new SmokingSession(DateTime.Now, sessionType);
        await _sessionService.AddSessionAsync(session);

        if (_dailyStatsRepository != null)
        {
            await _dailyStatsRepository.UpdateDailyStatsAsync(DateTime.Now);
        }
    }

    public async Task<List<SmokingSession>> GetTodaySessionsAsync()
    {
        return await _sessionService.GetSessionsByDateAsync(DateTime.Now);
    }

    public async Task<List<SmokingSession>> GetSessionsByDateAsync(DateTime date)
    {
        return await _sessionService.GetSessionsByDateAsync(date);
    }

    public async Task<List<SmokingSession>> GetSessionsInRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _sessionService.GetSessionsInRangeAsync(startDate, endDate);
    }

    public async Task<int> GetSmokingSessionsCountAsync(DateTime date)
    {
        var sessions = await _sessionService.GetSessionsByDateAsync(date);
        return sessions.Count(s => s.SessionType == "Smoking");
    }

    public async Task<int> GetPackPurchasesCountAsync(DateTime date)
    {
        var sessions = await _sessionService.GetSessionsByDateAsync(date);
        return sessions.Count(s => s.SessionType == "PackPurchase");
    }

    public async Task<List<SmokingSession>> GetAllSessionsAsync()
    {
        return await _sessionService.GetAllSessionsAsync();
    }

    public async Task ClearAllSessionsAsync()
    {
        await _sessionService.ClearAllSessionsAsync();
    }

    public string GetSessionsFilePath() => _sessionService.GetSessionsFilePath();
}