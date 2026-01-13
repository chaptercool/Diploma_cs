using Diploma_cs.Models;
using System.Globalization;
using System.Text;

namespace Diploma_cs.Data.Services;

public class CsvWeeklyStatsService
{
    private readonly string _weeklyStatsFilePath;
    private readonly object _lockObject = new object();

    public CsvWeeklyStatsService()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        _weeklyStatsFilePath = Path.Combine(appDataPath, "weekly_stats.csv");
    }

    public async Task AddWeeklyStatsAsync(WeeklyStats stats)
    {
        lock (_lockObject)
        {
            try
            {
                if (stats == null)
                    throw new ArgumentNullException(nameof(stats));

                bool fileExists = File.Exists(_weeklyStatsFilePath);

                using (var writer = new StreamWriter(_weeklyStatsFilePath, append: true, encoding: Encoding.UTF8))
                {
                    if (!fileExists)
                    {
                        writer.WriteLine("StartDate,EndDate,AvgConsumedWeek,AvgBoughtPacks,BoughtSum");
                    }

                    string line = $"{stats.StartDate:O},{stats.EndDate:O},{stats.AvgConsumedWeek:F2},{stats.AvgBoughtPacks:F2},{stats.BoughtSum:F2}";
                    writer.WriteLine(line);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding weekly stats to CSV: {ex.Message}", ex);
            }
        }
    }
    public async Task<List<WeeklyStats>> GetAllWeeklyStatsAsync()
    {
        var stats = new List<WeeklyStats>();

        lock (_lockObject)
        {
            try
            {
                if (!File.Exists(_weeklyStatsFilePath))
                    return stats;

                using (var reader = new StreamReader(_weeklyStatsFilePath, encoding: Encoding.UTF8))
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
                        if (parts.Length < 5)
                            continue;

                        try
                        {
                            if (DateTime.TryParse(parts[0], CultureInfo.InvariantCulture,
                                    DateTimeStyles.RoundtripKind, out var startDate) &&
                                DateTime.TryParse(parts[1], CultureInfo.InvariantCulture,
                                    DateTimeStyles.RoundtripKind, out var endDate) &&
                                float.TryParse(parts[2], CultureInfo.InvariantCulture, out var avgConsumed) &&
                                float.TryParse(parts[3], CultureInfo.InvariantCulture, out var avgBought) &&
                                float.TryParse(parts[4], CultureInfo.InvariantCulture, out var boughtSum))
                            {
                                stats.Add(new WeeklyStats
                                {
                                    StartDate = startDate.Date,
                                    EndDate = endDate.Date,
                                    AvgConsumedWeek = avgConsumed,
                                    AvgBoughtPacks = avgBought,
                                    BoughtSum = boughtSum
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
                throw new InvalidOperationException($"Error reading weekly stats from CSV: {ex.Message}", ex);
            }
        }

        return stats;
    }

    public async Task<WeeklyStats?> GetLatestWeeklyStatsAsync()
    {
        var allStats = await GetAllWeeklyStatsAsync();
        return allStats.LastOrDefault();
    }

    public async Task<List<WeeklyStats>> GetWeeklyStatsInRangeAsync(DateTime startDate, DateTime endDate)
    {
        var allStats = await GetAllWeeklyStatsAsync();
        return allStats
            .Where(s => s.StartDate >= startDate.Date && s.EndDate <= endDate.Date)
            .OrderBy(s => s.StartDate)
            .ToList();
    }

    public async Task ClearAllWeeklyStatsAsync()
    {
        lock (_lockObject)
        {
            try
            {
                if (File.Exists(_weeklyStatsFilePath))
                {
                    File.Delete(_weeklyStatsFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error clearing weekly stats: {ex.Message}", ex);
            }
        }
    }

    public string GetWeeklyStatsFilePath() => _weeklyStatsFilePath;
}