using System.Globalization;
using System.Text;

namespace Diploma_cs.Data.Services;

public class TargetRecord
{
    public DateTime Date { get; set; }
    public int Target { get; set; }
    public string CalculatedBy { get; set; } = "Initial"; // "Initial" or "ML"
    public float MlScore { get; set; }
}

public class CsvTargetService
{
    private readonly string _targetFilePath;
    private readonly object _lockObject = new object();

    public CsvTargetService()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        _targetFilePath = Path.Combine(appDataPath, "targets.csv");
    }

    public async Task AddTargetAsync(TargetRecord record)
    {
        lock (_lockObject)
        {
            try
            {
                if (record == null)
                    throw new ArgumentNullException(nameof(record));

                bool fileExists = File.Exists(_targetFilePath);

                using (var writer = new StreamWriter(_targetFilePath, append: true, encoding: Encoding.UTF8))
                {
                    if (!fileExists)
                    {
                        writer.WriteLine("Date,Target,CalculatedBy,MlScore");
                    }

                    string line = $"{record.Date:O},{record.Target},{record.CalculatedBy},{record.MlScore:F2}";
                    writer.WriteLine(line);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding target to CSV: {ex.Message}", ex);
            }
        }
    }

    public async Task<List<TargetRecord>> GetAllTargetsAsync()
    {
        var records = new List<TargetRecord>();

        lock (_lockObject)
        {
            try
            {
                if (!File.Exists(_targetFilePath))
                    return records;

                using (var reader = new StreamReader(_targetFilePath, encoding: Encoding.UTF8))
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
                                int.TryParse(parts[1], out var target) &&
                                float.TryParse(parts[3], CultureInfo.InvariantCulture, out var mlScore))
                            {
                                records.Add(new TargetRecord
                                {
                                    Date = date,
                                    Target = target,
                                    CalculatedBy = parts[2].Trim(),
                                    MlScore = mlScore
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
                throw new InvalidOperationException($"Error reading targets from CSV: {ex.Message}", ex);
            }
        }

        return records;
    }

    public async Task<TargetRecord?> GetCurrentTargetAsync()
    {
        var allTargets = await GetAllTargetsAsync();
        return allTargets.LastOrDefault();
    }

    public async Task<TargetRecord?> GetTargetForDateAsync(DateTime date)
    {
        var allTargets = await GetAllTargetsAsync();
        return allTargets
            .Where(t => t.Date.Date <= date.Date)
            .OrderByDescending(t => t.Date)
            .FirstOrDefault();
    }

    public async Task ClearAllTargetsAsync()
    {
        lock (_lockObject)
        {
            try
            {
                if (File.Exists(_targetFilePath))
                {
                    File.Delete(_targetFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error clearing targets: {ex.Message}", ex);
            }
        }
    }
    public string GetTargetFilePath() => _targetFilePath;
}