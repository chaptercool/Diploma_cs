using System.Globalization;
using System.Text;
using Diploma_cs.Data.Services;
using Diploma_cs.Models;

namespace Diploma_cs.Data.Services;

public class AchievementRepository
{
    private readonly CsvAchievementService _csvAchievementService;
    private readonly CsvUserAchievementService _csvUserAchievementService;

    public AchievementRepository()
    {
        _csvAchievementService = new CsvAchievementService();
        _csvUserAchievementService = new CsvUserAchievementService();
    }

    public async Task<Achievement?> GetAchievementByIdAsync(int achievementID)
    {
        var all = await GetAllAchievementsAsync();
        return all.FirstOrDefault(a => a.AchievementID == achievementID);
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        var achievements = await _csvAchievementService.GetAllAchievementsAsync();
        var unlockedIds = await _csvUserAchievementService.GetUnlockedAchievementsAsync();

        foreach (var achievement in achievements)
        {
            var unlockedRecord = unlockedIds.FirstOrDefault(u => u.AchievementID == achievement.AchievementID);
            if (unlockedRecord != null)
            {
                achievement.IsUnlocked = true;
                achievement.UnlockedDate = unlockedRecord.UnlockedDate;
            }
        }

        return achievements;
    }

    public async Task<bool> IsAchievementUnlockedAsync(int achievementID)
    {
        var unlockedIds = await _csvUserAchievementService.GetUnlockedAchievementsAsync();
        return unlockedIds.Any(u => u.AchievementID == achievementID);
    }

    public async Task SaveUnlockedAchievementAsync(int achievementID, DateTime unlockedDate)
    {
        await _csvUserAchievementService.SaveAchievementAsync(achievementID, unlockedDate);
    }
}

public class CsvAchievementService
{
    private const string FileName = "achievements.csv";
    private readonly string _filePath;

    public CsvAchievementService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
    }

    public async Task<List<Achievement>> GetAllAchievementsAsync()
    {
        var achievements = new List<Achievement>();

        if (!File.Exists(_filePath))
            return achievements;

        try
        {
            var bytes = await File.ReadAllBytesAsync(_filePath);

            string content = Encoding.UTF8.GetString(bytes);
            bool hasReplacement = content.IndexOf('\uFFFD') >= 0;

            if (hasReplacement)
            {
                try
                {
                    var cp1250 = Encoding.GetEncoding(1250);
                    content = cp1250.GetString(bytes);
                }
                catch
                {
                }
            }

            var lines = content.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var parts = lines[i].Split(';');
                if (parts.Length >= 5)
                {
                    var id = parts[0].Trim();
                    var name = parts[1].Trim();
                    var statement = parts[2].Trim();
                    var descUnlocked = parts[3].Trim();
                    var descLocked = parts[4].Trim();

                    if (int.TryParse(id, out int achievementID))
                    {
                        achievements.Add(new Achievement
                        {
                            AchievementID = achievementID,
                            Name = name,
                            Statement = statement,
                            DescriptionUnlocked = descUnlocked == "None" ? string.Empty : descUnlocked,
                            DescriptionLocked = descLocked == "None" ? string.Empty : descLocked
                        });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Loaded {achievements.Count} achievements from CSV");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading achievements: {ex.Message}");
        }

        return achievements;
    }
}

public class CsvUserAchievementService
{
    private const string FileName = "user_achievements.csv";
    private readonly string _filePath;

    public CsvUserAchievementService()
    {
        _filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
    }

    public async Task<List<UserAchievement>> GetUnlockedAchievementsAsync()
    {
        var achievements = new List<UserAchievement>();

        if (!File.Exists(_filePath))
            return achievements;

        try
        {
            var lines = await File.ReadAllLinesAsync(_filePath, Encoding.UTF8);

            int startIndex = lines.Length > 0 && lines[0].Contains("AchievementID") ? 1 : 0;

            for (int i = startIndex; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                    continue;

                var parts = lines[i].Split(';');
                if (parts.Length >= 2)
                {
                    if (int.TryParse(parts[0].Trim(), out int id) &&
                        DateTime.TryParse(parts[1].Trim(), CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out var date))
                    {
                        achievements.Add(new UserAchievement
                        {
                            AchievementID = id,
                            UnlockedDate = date
                        });
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine($"Loaded {achievements.Count} unlocked achievements");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading user achievements: {ex.Message}");
        }

        return achievements;
    }

    public async Task SaveAchievementAsync(int achievementID, DateTime unlockedDate)
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                await File.WriteAllTextAsync(_filePath, "AchievementID;UnlockedDate\n", Encoding.UTF8);
            }

            var existing = await GetUnlockedAchievementsAsync();
            if (existing.Any(a => a.AchievementID == achievementID))
                return;

            var line = $"{achievementID};{unlockedDate:O}";
            await File.AppendAllTextAsync(_filePath, line + Environment.NewLine, Encoding.UTF8);
            
            System.Diagnostics.Debug.WriteLine($"Achievement {achievementID} unlocked and saved");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving achievement: {ex.Message}");
        }
    }
}