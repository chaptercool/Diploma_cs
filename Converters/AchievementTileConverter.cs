using Diploma_cs.Models;
using Microsoft.Maui.Controls;
using System.Globalization;
using System;
using System.IO;
using Microsoft.Maui.Storage;

namespace Diploma_cs.Converters;

public class AchievementTileConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Achievement achievement)
        {
            return achievement.IsUnlocked ? "unlock.png" : "lock.png";
        }
        return "lock.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class AchievementStatusColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Achievement achievement)
        {
            return achievement.IsUnlocked ? Color.FromArgb("#4CAF50") : Color.FromArgb("#9E9E9E");
        }
        return Colors.Gray;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class AchievementStatusTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is Achievement achievement)
        {
            return achievement.IsUnlocked ? "Odblokowano" : "Zablokowana";
        }
        return "Zablokowana";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

public class AchievementIconConverter : IValueConverter
{
    private const string FallbackImage = "achievement_placeholder.png";

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        try
        {
            var fileName = value as string;
            if (string.IsNullOrWhiteSpace(fileName))
                return ImageSource.FromFile(FallbackImage);

            // Prefer user-provided files in app data directory (fast file existence check)
            var appDataPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            if (File.Exists(appDataPath))
            {
                // Return ImageSource from full path (works for files stored in app data)
                return ImageSource.FromFile(appDataPath);
            }

            // For packaged resources, ImageSource.FromFile with the resource filename is efficient
            // and does not require opening streams synchronously. Try a few likely locations by name only.
            string[] packageCandidates =
            {
                fileName,
                Path.GetFileName(fileName),
                $"Data/Misc/{fileName}",
                $"Data/Images/{fileName}",
                $"Resources/Images/{fileName}"
            };

            foreach (var candidate in packageCandidates)
            {
                try
                {
                    // ImageSource.FromFile is lightweight for packaged images and will resolve the resource.
                    var img = ImageSource.FromFile(candidate);
                    if (img != null)
                        return img;
                }
                catch
                {
                    // ignore and try next
                }
            }

            // Last resort: try using the bare filename
            try
            {
                return ImageSource.FromFile(fileName);
            }
            catch
            {
            }

            return ImageSource.FromFile(FallbackImage);
        }
        catch
        {
            return ImageSource.FromFile(FallbackImage);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}