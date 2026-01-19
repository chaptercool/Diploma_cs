using Diploma_cs.Data.Repositories;
using Diploma_cs.Data.Services;
using Diploma_cs.Models;

namespace Diploma_cs.Data.Services;

public class AppDataService
{
    private readonly UserProfileRepository _userProfileRepository;
    private readonly SessionRepository _sessionRepository;
    private readonly DailyStatsRepository _dailyStatsRepository;
    private readonly WeeklyStatsRepository _weeklyStatsRepository;
    private readonly TargetRepository _targetRepository;

    private readonly CsvUserProfileService _csvUserProfileService;
    private readonly CsvSessionService _csvSessionService;
    private readonly CsvDailyStatsService _csvDailyStatsService;
    private readonly CsvWeeklyStatsService _csvWeeklyStatsService;
    private readonly CsvTargetService _csvTargetService;

    private bool _isInitialized = false;

    public AppDataService()
    {
        _csvUserProfileService = new CsvUserProfileService();
        _csvSessionService = new CsvSessionService();
        _csvDailyStatsService = new CsvDailyStatsService();
        _csvWeeklyStatsService = new CsvWeeklyStatsService();
        _csvTargetService = new CsvTargetService();

        _weeklyStatsRepository = new WeeklyStatsRepository(_csvWeeklyStatsService);
        _targetRepository = new TargetRepository(_csvTargetService, _weeklyStatsRepository);

        _sessionRepository = new SessionRepository(_csvSessionService);

        _dailyStatsRepository = new DailyStatsRepository(
            _csvDailyStatsService,
            _sessionRepository,
            _targetRepository);

        _sessionRepository.SetDailyStatsRepository(_dailyStatsRepository);

        _userProfileRepository = new UserProfileRepository(_csvUserProfileService, _targetRepository);
    }

    public async Task InitializeAsync()
    {
        if (_isInitialized)
            return;

        try
        {
            _isInitialized = true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to initialize AppDataService", ex);
        }
    }

    #region Setup Operations

    public bool IsSetupComplete()
    {
        return _userProfileRepository.IsSetupComplete();
    }

    public async Task CompleteSetupAsync(string name, int age, string gender, int dailyConsumption, float packPrice)
    {
        try
        {
            await _userProfileRepository.SaveUserProfileAsync(name, age, gender, dailyConsumption, packPrice);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to complete setup", ex);
        }
    }

    public async Task<UserProfile?> GetUserProfileAsync()
    {
        try
        {
            return await _userProfileRepository.GetUserProfileAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to retrieve user profile", ex);
        }
    }

    #endregion

    #region Session Operations

    public async Task RegisterSmokingSessionAsync()
    {
        try
        {
            await _sessionRepository.RegisterSessionAsync("Smoking");
            await CheckAndCalculateWeeklyStatsAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to register smoking session", ex);
        }
    }

    public async Task RegisterPackPurchaseAsync()
    {
        try
        {
            await _sessionRepository.RegisterSessionAsync("PackPurchase");
            await CheckAndCalculateWeeklyStatsAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to register pack purchase", ex);
        }
    }

    public async Task<int> GetTodaySessionsCountAsync()
    {
        try
        {
            return await _sessionRepository.GetSmokingSessionsCountAsync(DateTime.Now);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get today's session count", ex);
        }
    }

    public async Task<int> GetTodayPacksCountAsync()
    {
        try
        {
            return await _sessionRepository.GetPackPurchasesCountAsync(DateTime.Now);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get today's pack count", ex);
        }
    }

    #endregion

    #region Daily Statistics Operations

    public async Task<DailyStats?> GetTodayStatsAsync()
    {
        try
        {
            return await _dailyStatsRepository.GetDailyStatsByDateAsync(DateTime.Now);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get today's stats", ex);
        }
    }

    public async Task<DailyStats?> GetDailyStatsAsync(DateTime date)
    {
        try
        {
            return await _dailyStatsRepository.GetDailyStatsByDateAsync(date);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get daily stats for {date:d}", ex);
        }
    }

    public async Task<List<DailyStats>> GetDailyStatsRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            return await _dailyStatsRepository.GetDailyStatsInRangeAsync(startDate, endDate);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get daily stats range", ex);
        }
    }

    public async Task<DailyStats.DayStatus?> GetDayStatusAsync(DateTime date)
    {
        try
        {
            return await _dailyStatsRepository.GetDayStatusAsync(date);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get day status for {date:d}", ex);
        }
    }

    #endregion

    #region Target Operations

    public async Task<int?> GetCurrentTargetAsync()
    {
        try
        {
            return await _targetRepository.GetCurrentTargetAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get current target", ex);
        }
    }

    public async Task<int?> GetTargetForDateAsync(DateTime date)
    {
        try
        {
            return await _targetRepository.GetTargetForDateAsync(date);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get target for {date:d}", ex);
        }
    }

    public async Task<bool> IsSuccessfullyQuitAsync()
    {
        try
        {
            return await _targetRepository.IsSuccessfullyQuitAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to check quit status", ex);
        }
    }

    #endregion

    #region Weekly Statistics and ML Operations

    public async Task<bool> CheckAndCalculateWeeklyStatsAsync()
    {
        try
        {
            var profile = await _userProfileRepository.GetUserProfileAsync();
            if (profile == null) return false;

            var daysSinceSetup = (DateTime.Now - profile.SetupCompletedDate).TotalDays;

            if (daysSinceSetup >= 7)
            {
                var weeklyStats = await _weeklyStatsRepository.CalculateAndSaveWeeklyStatsAsync();

                if (weeklyStats != null)
                {
                    var newTarget = await _targetRepository.CalculateAndSaveMLTargetAsync();
                    return true;
                }
            }

            return false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to check and calculate weekly stats", ex);
        }
    }

    public async Task<WeeklyStats?> GetLatestWeeklyStatsAsync()
    {
        try
        {
            return await _weeklyStatsRepository.GetLatestWeeklyStatsAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get latest weekly stats", ex);
        }
    }

    public async Task<List<WeeklyStats>> GetAllWeeklyStatsAsync()
    {
        try
        {
            return await _weeklyStatsRepository.GetAllWeeklyStatsAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to get all weekly stats", ex);
        }
    }

    #endregion

    #region Data Access for UI

    public async Task<DayDetailInfo> GetDayDetailAsync(DateTime date)
    {
        try
        {
            var stats = await _dailyStatsRepository.GetDailyStatsByDateAsync(date);
            var sessions = await _sessionRepository.GetSessionsByDateAsync(date);

            return new DayDetailInfo
            {
                Date = date,
                SessionsCount = sessions.Count(s => s.SessionType == "Smoking"),
                PacksCount = sessions.Count(s => s.SessionType == "PackPurchase"),
                Target = stats?.DailyTarget ?? 0,
                Status = stats?.GetDayStatus() ?? DailyStats.DayStatus.Ok,
                Sessions = sessions
            };
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get day detail for {date:d}", ex);
        }
    }

    public async Task<List<DailyStats>> GetMonthStatsAsync(int year, int month)
    {
        try
        {
            var firstDay = new DateTime(year, month, 1);
            var lastDay = firstDay.AddMonths(1).AddDays(-1);

            return await _dailyStatsRepository.GetDailyStatsInRangeAsync(firstDay, lastDay);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to get month stats for {year}-{month:D2}", ex);
        }
    }

    #endregion

    #region Data Reset (Use with Caution)

    public async Task ResetAllDataAsync()
    {
        try
        {
            await _userProfileRepository.ResetAllUserDataAsync();
            await _sessionRepository.ClearAllSessionsAsync();
            await _dailyStatsRepository.ClearAllDailyStatsAsync();
            await _weeklyStatsRepository.ClearAllWeeklyStatsAsync();
            await _targetRepository.ClearAllTargetsAsync();
            _isInitialized = false;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to reset all data", ex);
        }
    }

    #endregion

    #region Debugging & Export

    public Dictionary<string, string> GetAllDataFilePaths()
    {
        return new Dictionary<string, string>
        {
            { "UserProfile", _userProfileRepository.GetProfileFilePath() },
            { "Sessions", _sessionRepository.GetSessionsFilePath() },
            { "DailyStats", _dailyStatsRepository.GetDailyStatsFilePath() },
            { "WeeklyStats", _weeklyStatsRepository.GetWeeklyStatsFilePath() },
            { "Targets", _targetRepository.GetTargetFilePath() }
        };
    }

    #endregion

    public async Task<int> RebuildDailyStatsFromSessionsAsync(
        bool clearExistingDailyStats = true,
        DateTime? startDate = null,
        DateTime? endDate = null)
    {
        try
        {
            if (clearExistingDailyStats)
                await _dailyStatsRepository.ClearAllDailyStatsAsync();

            var sessions = await _sessionRepository.GetAllSessionsAsync();
            if (sessions.Count == 0)
                return 0;

            var distinctDays = sessions
                .Select(s => s.SessionTime.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            var from = (startDate?.Date) ?? distinctDays.First();
            var to = (endDate?.Date) ?? distinctDays.Last();

            int rebuilt = 0;

            foreach (var day in distinctDays)
            {
                if (day < from || day > to)
                    continue;

                await _dailyStatsRepository.UpdateDailyStatsAsync(day);
                rebuilt++;
            }

            return rebuilt;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to rebuild daily stats from sessions", ex);
        }
    }
}

public class DayDetailInfo
{
    public DateTime Date { get; set; }
    public int SessionsCount { get; set; }
    public int PacksCount { get; set; }
    public int Target { get; set; }
    public DailyStats.DayStatus Status { get; set; }
    public List<SmokingSession> Sessions { get; set; } = new();

    public Color GetStatusColor()
    {
        return Status switch
        {
            DailyStats.DayStatus.Ok => Color.FromArgb("#9EC1FB"),
            DailyStats.DayStatus.Target => Color.FromArgb("#4692E3"),
            DailyStats.DayStatus.Exceeded => Color.FromArgb("#CC1714"),
            _ => Color.FromArgb("#E6E8EB")
        };
    }

    public string GetStatusText()
    {
        return Status switch
        {
            DailyStats.DayStatus.Ok => "Poni¿ej celu",
            DailyStats.DayStatus.Target => "Osi¹gniêto cel",
            DailyStats.DayStatus.Exceeded => "Przekroczono cel",
            _ => "Brak danych"
        };
    }
}