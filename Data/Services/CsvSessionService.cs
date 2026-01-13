using Diploma_cs.Models;
using System.Globalization;
using System.Text;

namespace Diploma_cs.Data.Services;

public class CsvSessionService
{
    private readonly string _sessionsFilePath;
    private readonly object _lockObject = new object();

    public CsvSessionService()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        _sessionsFilePath = Path.Combine(appDataPath, "sessions.csv");
    }

    public async Task AddSessionAsync(SmokingSession session)
    {
        lock (_lockObject)
        {
            try
            {
                if (session == null)
                    throw new ArgumentNullException(nameof(session));

                bool fileExists = File.Exists(_sessionsFilePath);

                using (var writer = new StreamWriter(_sessionsFilePath, append: true, encoding: Encoding.UTF8))
                {
                    if (!fileExists)
                    {
                        writer.WriteLine("SessionTime,SessionType");
                    }

                    string line = $"{session.SessionTime:O},{session.SessionType}";
                    writer.WriteLine(line);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error adding session to CSV: {ex.Message}", ex);
            }
        }
    }

    public async Task<List<SmokingSession>> GetAllSessionsAsync()
    {
        var sessions = new List<SmokingSession>();

        lock (_lockObject)
        {
            try
            {
                if (!File.Exists(_sessionsFilePath))
                    return sessions;

                using (var reader = new StreamReader(_sessionsFilePath, encoding: Encoding.UTF8))
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
                        if (parts.Length < 2)
                            continue;

                        try
                        {
                            if (DateTime.TryParse(parts[0], CultureInfo.InvariantCulture,
                                DateTimeStyles.RoundtripKind, out var sessionTime))
                            {
                                sessions.Add(new SmokingSession
                                {
                                    Id = sessions.Count + 1,
                                    SessionTime = sessionTime,
                                    SessionType = parts[1].Trim()
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
                throw new InvalidOperationException($"Error reading sessions from CSV: {ex.Message}", ex);
            }
        }

        return sessions;
    }

    public async Task<List<SmokingSession>> GetSessionsByDateAsync(DateTime date)
    {
        var allSessions = await GetAllSessionsAsync();
        return allSessions.Where(s => s.SessionTime.Date == date.Date).ToList();
    }

    public async Task<List<SmokingSession>> GetSessionsInRangeAsync(DateTime startDate, DateTime endDate)
    {
        var allSessions = await GetAllSessionsAsync();
        return allSessions
            .Where(s => s.SessionTime.Date >= startDate.Date && s.SessionTime.Date <= endDate.Date)
            .ToList();
    }

    public async Task ClearAllSessionsAsync()
    {
        lock (_lockObject)
        {
            try
            {
                if (File.Exists(_sessionsFilePath))
                {
                    File.Delete(_sessionsFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error clearing sessions: {ex.Message}", ex);
            }
        }
    }
    public string GetSessionsFilePath() => _sessionsFilePath;
}