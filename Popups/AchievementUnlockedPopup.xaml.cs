using Diploma_cs.Models;

namespace Diploma_cs.Popups;

public partial class AchievementUnlockedPopup : ContentPage
{
    public AchievementUnlockedPopup(Achievement achievement)
    {
        InitializeComponent();
        LoadAchievementData(achievement);
    }

    private void LoadAchievementData(Achievement achievement)
    {
        AchievementIcon.Source = ImageSource.FromFile(achievement.GetIconFileName());
        AchievementName.Text = achievement.Name;
        AchievementDescription.Text = achievement.DescriptionUnlocked;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}