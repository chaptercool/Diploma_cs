using System.Windows.Input;
using Diploma_cs.Models;
using Microsoft.Maui.Controls;

namespace Diploma_cs.ViewModels;

public class AchievementTileViewModel
{
    public Achievement Achievement { get; set; }
    public Color IconBackgroundColor => Color.FromArgb("#9EC1FB");
    public Color StatusColor => Achievement.IsUnlocked ? Color.FromArgb("#4CAF50") : Color.FromArgb("#9E9E9E");
    public string StatusText => Achievement.IsUnlocked ? "Odblokowano" : "Zablokowana";
    public string LockIconSource => Achievement.IsUnlocked ? "unlock.png" : "lock.png";
    public string AchievementIconSource => Achievement.GetIconFileName();

    public AchievementTileViewModel(Achievement achievement)
    {
        Achievement = achievement;
    }
}