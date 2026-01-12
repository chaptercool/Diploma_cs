using Diploma_cs.Data.Services;
using Diploma_cs.Data.Services.Achievements;
using Diploma_cs.Models;
using Diploma_cs.Popups;
using Diploma_cs.Services;
using System.Diagnostics;

namespace Diploma_cs
{
    public partial class MainPage : ContentPage
    {
        private readonly AppDataService _appDataService;
        private readonly AchievementService _achievementService;
        private int _currentTarget = 0;
        private bool _achievementsCheckedThisSession = false;

        public MainPage()
        {
            InitializeComponent();
            _appDataService = ServiceHelper.GetService<AppDataService>();
            _achievementService = ServiceHelper.GetService<AchievementService>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshPageDataAsync();

            if (!_achievementsCheckedThisSession)
            {
                _achievementsCheckedThisSession = true;
                await CheckAndShowNewAchievementsAsync();
            }
        }

        private async Task CheckAndShowNewAchievementsAsync()
        {
            try
            {
                var newlyUnlocked = await _achievementService.CheckAllAchievementsAsync();
                System.Diagnostics.Debug.WriteLine($"MainPage: Achievement check found {newlyUnlocked.Count} newly unlocked");

                if (newlyUnlocked.Count > 0)
                {
                    foreach (var achievement in newlyUnlocked)
                    {
                        System.Diagnostics.Debug.WriteLine($"MainPage: Showing unlock popup for achievement: {achievement.Name}");
                        await Navigation.PushModalAsync(new AchievementUnlockedPopup(achievement));
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"MainPage.CheckAndShowNewAchievementsAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private async Task RefreshPageDataAsync()
        {
            try
            {
                var currentTarget = await _appDataService.GetCurrentTargetAsync();
                _currentTarget = currentTarget ?? 0;

                var todaySessionsCount = await _appDataService.GetTodaySessionsCountAsync();

                UpdateMainCounter(todaySessionsCount, _currentTarget);

                await LoadRecentAchievementsAsync();

                bool isQuit = await _appDataService.IsSuccessfullyQuitAsync();
                if (isQuit)
                {
                    await DisplayAlert("Gratulacje!", "Udało Ci się rzucić palenie!", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load page data: {ex.Message}", "OK");
            }
        }

        private void UpdateMainCounter(int sessionsCount, int target)
        {
            if (TargetLabel != null)
            {
                TargetLabel.Text = target.ToString();
                
                if (sessionsCount < target)
                {
                    TargetLabel.TextColor = Color.FromArgb("#9EC1FB");
                }
                else if (sessionsCount == target)
                {
                    TargetLabel.TextColor = Color.FromArgb("#4692E3");
                }
                else
                {
                    TargetLabel.TextColor = Color.FromArgb("#CC1714");
                }
            }

            if (SessionCountLabel != null)
            {
                SessionCountLabel.Text = $"Dotychczasowo zarejestrowano sesji: {sessionsCount}";
            }
        }

        private async Task LoadRecentAchievementsAsync()
        {
            try
            {
                var achievements = await _achievementService.GetAllAchievementsAsync();
                
                if (achievements == null || achievements.Count == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No achievements found");
                    return;
                }

                var recentAchievements = achievements
                    .Where(a => a.IsUnlocked)
                    .OrderByDescending(a => a.UnlockedDate)
                    .Take(2)
                    .ToList();

                if (recentAchievements.Count < 2)
                {
                    var lockedAchievements = achievements
                        .Where(a => !a.IsUnlocked)
                        .Take(2 - recentAchievements.Count)
                        .ToList();
                    recentAchievements.AddRange(lockedAchievements);
                }

                AchievementsCollectionView.ItemsSource = recentAchievements.Take(2).ToList();
                AchievementsCollectionView.SelectionChangedCommand = new Command<Achievement>(async (achievement) =>
                {
                    if (achievement != null)
                    {
                        await Navigation.PushModalAsync(new AchievementDetailPopup(achievement));
                        AchievementsCollectionView.SelectedItem = null;
                    }
                });

                System.Diagnostics.Debug.WriteLine($"MainPage: Loaded {recentAchievements.Count} recent achievements");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading achievements: {ex.Message}");
            }
        }

        private async void OnAchievementsButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AchievementsPage());
        }
    }
}
