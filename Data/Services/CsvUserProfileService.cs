using Diploma_cs.Models;
using System.Globalization;
using System.Text;

namespace Diploma_cs.Data.Services;

public class CsvUserProfileService
{
    private readonly string _profileFilePath;
    private readonly object _lockObject = new object();

    public CsvUserProfileService()
    {
        var appDataPath = FileSystem.AppDataDirectory;
        _profileFilePath = Path.Combine(appDataPath, "user_profile.csv");
    }

    public async Task SaveUserProfileAsync(UserProfile profile)
    {
        lock (_lockObject)
        {
            try
            {
                if (profile == null)
                    throw new ArgumentNullException(nameof(profile));

                using (var writer = new StreamWriter(_profileFilePath, append: false, encoding: Encoding.UTF8))
                {
                    writer.WriteLine("Name,Age,Gender,DailyConsumption,PackPrice,SetupCompletedDate,InitialTarget");

                    string line = $"\"{profile.Name}\",{profile.Age},\"{profile.Gender}\",{profile.DailyConsumption},{profile.PackPrice:F2},{profile.SetupCompletedDate:O},{profile.InitialTarget}";
                    writer.WriteLine(line);
                    writer.Flush();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error saving user profile to CSV: {ex.Message}", ex);
            }
        }
    }

    public async Task<UserProfile?> GetUserProfileAsync()
    {
        lock (_lockObject)
        {
            try
            {
                if (!File.Exists(_profileFilePath))
                    return null;

                using (var reader = new StreamReader(_profileFilePath, encoding: Encoding.UTF8))
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

                        try
                        {
                            var parts = ParseCsvLine(line);
                            if (parts.Length < 7)
                                continue;

                            if (int.TryParse(parts[1], out var age) &&
                                int.TryParse(parts[3], out var dailyConsumption) &&
                                float.TryParse(parts[4], CultureInfo.InvariantCulture, out var packPrice) &&
                                DateTime.TryParse(parts[5], CultureInfo.InvariantCulture, 
                                    DateTimeStyles.RoundtripKind, out var setupDate) &&
                                int.TryParse(parts[6], out var initialTarget))
                            {
                                return new UserProfile
                                {
                                    Name = parts[0],
                                    Age = age,
                                    Gender = parts[2],
                                    DailyConsumption = dailyConsumption,
                                    PackPrice = packPrice,
                                    SetupCompletedDate = setupDate,
                                    InitialTarget = initialTarget
                                };
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
                throw new InvalidOperationException($"Error reading user profile from CSV: {ex.Message}", ex);
            }
        }

        return null;
    }

    public bool ProfileExists()
    {
        return File.Exists(_profileFilePath);
    }

    public async Task DeleteProfileAsync()
    {
        lock (_lockObject)
        {
            try
            {
                if (File.Exists(_profileFilePath))
                {
                    File.Delete(_profileFilePath);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error deleting user profile: {ex.Message}", ex);
            }
        }
    }

    public string GetProfileFilePath() => _profileFilePath;

    private string[] ParseCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        bool inQuotes = false;

        foreach (char c in line)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        result.Add(current.ToString());
        return result.ToArray();
    }
}