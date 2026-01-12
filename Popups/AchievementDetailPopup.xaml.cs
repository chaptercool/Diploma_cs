using Diploma_cs.Models;

namespace Diploma_cs.Popups;

public partial class AchievementDetailPopup : ContentPage
{
    public AchievementDetailPopup(Achievement achievement)
    {
        InitializeComponent();
        LoadAchievementDetails(achievement);

        var tapGestureRecognizer = new TapGestureRecognizer();
        tapGestureRecognizer.Tapped += async (s, e) => await Close();
        CloseArea.GestureRecognizers.Add(tapGestureRecognizer);
    }

    private void LoadAchievementDetails(Achievement achievement)
    {
        AchievementIcon.Source = ImageSource.FromFile(achievement.GetIconFileName());
        AchievementTitle.Text = achievement.Name;
        AchievementDescription.Text = achievement.GetDescription();

        if (achievement.IsUnlocked)
        {
            StatusLabel.Text = "Odblokowano";
            StatusLabel.TextColor = Color.FromArgb("#4CAF50");
            StatusIcon.Source = "unlock.png";
        }
        else
        {
            StatusLabel.Text = "Zablokowana";
            StatusLabel.TextColor = Color.FromArgb("#9E9E9E");
            StatusIcon.Source = "lock.png";
        }
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await Close();
    }

    private async Task Close()
    {
        await Navigation.PopModalAsync();
    }
}