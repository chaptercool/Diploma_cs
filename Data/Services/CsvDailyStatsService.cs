using Diploma_cs.Models;
using System.Globalization;
using System.Text;

namespace Diploma_cs.Data.Services;

public class CsvDailyStatsService
{
    private readonly string _dailyStatsFilePath;
    private readonly object _lockObject = new object();

    public CsvDailyStatsService()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        _dailyStatsFilePath = Path.Combine(appDataPath, "daily_stats.csv");
    }
    public async Task SaveDailyStatsAsync(DailyStats stats)
    {
        lock (_lockObject)
        {
            try
            {
                if (stats == null)
                    throw new ArgumentNullException(nameof(stats));

                var allStats = ReadAllDailyStats();
                
                allStats.RemoveAll(s => s.Date.Date == stats.Date.Date);
                
                allStats.Add(stats);
                
                allStats = allStats.OrderBy(s => s.Date).ToList();

                WriteDailyStatsToFile(allStats);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving daily stats to CSV: {ex.Message}", ex);
            }
        }
    }

    public async Task<List<DailyStats>> GetAllDailyStatsAsync()
    {
        lock (_lockObject)
        {
            return ReadAllDailyStats();
        }
    }

    public async Task<DailyStats?> GetDailyStatsByDateAsync(DateTime date)
    {
        var allStats = await GetAllDailyStatsAsync();
        return allStats.FirstOrDefault(s => s.Date.Date == date.Date);
    }
    public async Task<List<DailyStats>> GetDailyStatsInRangeAsync(DateTime startDate, DateTime endDate)
    {
        var allStats = await GetAllDailyStatsAsync();
        return allStats
            .Where(s => s.Date.Date >= startDate.Date && s.Date.Date <= endDate.Date)
            .OrderBy(s => s.Date)
            .ToList();
    }

    public async Task<DailyStats?> GetLatestDailyStatsAsync()
    {
        var allStats = await GetAllDailyStatsAsync();
        return allStats.LastOrDefault();
    }

    private List<DailyStats> ReadAllDailyStats()
    {
        var stats = new List<DailyStats>();

        try
        {
            if (!File.Exists(_dailyStatsFilePath))
                return stats;

            using (var reader = new StreamReader(_dailyStatsFilePath, encoding: Encoding.UTF8))
            {
                string? line;
                int lineNumber = 0;

                while ((line = reader.ReadLine()) != null)
                {
                    lineNumber++;
                    if (lineNumber == 1)
                        continue;

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    var parts = line.Split(',');
                    if (parts.Length < 4)
                        continue;

                    try
                    {
                        if (DateTime.TryParse(parts[0], CultureInfo.InvariantCulture, 
                                DateTimeStyles.RoundtripKind, out var date) &&
                            int.TryParse(parts[1], out var sessionsCount) &&
                            int.TryParse(parts[2], out var packsCount) &&
                            int.TryParse(parts[3], out var dailyTarget))
                        {
                            stats.Add(new DailyStats
                            {
                                Date = date.Date,
                                SessionsCount = sessionsCount,
                                PacksCount = packsCount,
                                DailyTarget = dailyTarget
                            });
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error reading daily stats from CSV: {ex.Message}", ex);
        }

        return stats;
    }

    private void WriteDailyStatsToFile(List<DailyStats> stats)
    {
        using (var writer = new StreamWriter(_dailyStatsFilePath, append: false, encoding: Encoding.UTF8))
        {
            writer.WriteLine("Date,SessionsCount,PacksCount,DailyTarget");

            foreach (var stat in stats.OrderBy(s => s.Date))
            {
                string line = $"{stat.Date:O},{stat.SessionsCount},{stat.PacksCount},{stat.DailyTarget}";
                writer.WriteLine(line);
            }

            writer.Flush();
        }
    }

    public async Task ClearAllDailyStatsAsync()
    {
        lock (_lockObject)
        {
            try
            {
                if (File.Exists(_dailyStatsFilePath))
                {
                    File.Delete(_dailyStatsFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error clearing daily stats: {ex.Message}", ex);
            }
        }
    }

    public string GetDailyStatsFilePath() => _dailyStatsFilePath;
}