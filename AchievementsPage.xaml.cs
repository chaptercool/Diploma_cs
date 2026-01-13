using Diploma_cs.Data.Services.Achievements;
using Diploma_cs.Models;
using Diploma_cs.Popups;
using Diploma_cs.Services;

namespace Diploma_cs;

public partial class AchievementsPage : ContentPage
{
    private readonly AchievementService _achievementService;

    public AchievementsPage()
    {
        InitializeComponent();
        _achievementService = ServiceHelper.GetService<AchievementService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAchievementsAsync();
    }

    private async Task LoadAchievementsAsync()
    {
        try
        {
            LoadingIndicator.IsVisible = true;
            LoadingIndicator.IsRunning = true;

            var achievements = await _achievementService.GetAllAchievementsAsync();

            System.Diagnostics.Debug.WriteLine($"AchievementsPage: Loaded {achievements?.Count ?? 0} achievements");

            if (achievements == null || achievements.Count == 0)
            {
                ProgressLabel.Text = "Brak osi¹gniêæ - achievements.csv nie zosta³ za³adowany";
                LoadingIndicator.IsVisible = false;
                LoadingIndicator.IsRunning = false;
                return;
            }

            var unlockedCount = achievements.Count(a => a.IsUnlocked);
            var totalCount = achievements.Count;

            ProgressLabel.Text = $"Odblokowano: {unlockedCount}/{totalCount}";

            AchievementsCollectionView.ItemsSource = achievements;
            AchievementsCollectionView.SelectionChangedCommand = new Command<Achievement>(async (achievement) =>
            {
                if (achievement != null)
                {
                    await Navigation.PushModalAsync(new AchievementDetailPopup(achievement));
                    AchievementsCollectionView.SelectedItem = null;
                }
            });

            System.Diagnostics.Debug.WriteLine($"AchievementsPage: Bound {achievements.Count} achievements to CollectionView");

            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"AchievementsPage Error: {ex.Message}");
            await DisplayAlert("Error", $"Failed to load achievements: {ex.Message}", "OK");
            LoadingIndicator.IsVisible = false;
            LoadingIndicator.IsRunning = false;
        }
    }
}