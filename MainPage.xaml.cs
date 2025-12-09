using Diploma_cs.Data.Services;
using Diploma_cs.Services;
using Diploma_cs.Models;

namespace Diploma_cs
{
    public partial class MainPage : ContentPage
    {
        private readonly AppDataService _appDataService;
        private int _currentTarget = 0;

        public MainPage()
        {
            InitializeComponent();
            _appDataService = ServiceHelper.GetService<AppDataService>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await RefreshPageDataAsync();
        }

        private async Task RefreshPageDataAsync()
        {
            try
            {
                var currentTarget = await _appDataService.GetCurrentTargetAsync();
                _currentTarget = currentTarget ?? 0;

                var todaySessionsCount = await _appDataService.GetTodaySessionsCountAsync();

                UpdateMainCounter(todaySessionsCount, _currentTarget);

                bool isQuit = await _appDataService.IsSuccessfullyQuitAsync();
                if (isQuit)
                {
                    await DisplayAlert("Gratulacje!", "Udało Ci się rzucić palenie!", "OK");
                }

                // await UpdateAchievementsAsync();
                // await UpdateStatisticsAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to load page data: {ex.Message}", "OK");
            }
        }

        private void UpdateMainCounter(int sessionsCount, int target)
        {
            Label SessionCount = this.FindByName<Label>("SessionCountLabel");
            if (MainCounterBlock?.Children.Count >= 2)
            {
                if (MainCounterBlock.Children[0] is Label counterLabel)
                {
                    counterLabel.Text = target.ToString();
                    
                    if (sessionsCount < target)
                    {
                        counterLabel.TextColor = Color.FromArgb("#9EC1FB");
                    }
                    else if (sessionsCount == target)
                    {
                        counterLabel.TextColor = Color.FromArgb("#4692E3");
                    }
                    else
                    {
                        counterLabel.TextColor = Color.FromArgb("#CC1714");
                    }
                }

                if (SessionCount != null)
                {
                    SessionCount.Text = $"Dotychczasowo zarejestrowano sesji: {sessionsCount}";
                }
            }
        }

        //private async Task UpdateAchievementsAsync()
        //{
        //    await Task.CompletedTask;
        //}

        //private async Task UpdateStatisticsAsync()
        //{
        //    try
        //    {
        //        var latestWeeklyStats = await _appDataService.GetLatestWeeklyStatsAsync();
        //        if (latestWeeklyStats != null)
        //        {
        //            // nic
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine($"Error updating statistics: {ex.Message}");
        //    }
        //}
    }
}
