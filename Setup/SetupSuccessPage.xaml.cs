using System;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Diploma_cs.Services;
using Diploma_cs.Data.Services;
using Diploma_cs.Data.Services.Achievements;
using Diploma_cs.Popups;

namespace Diploma_cs.Setup;

public partial class SetupSuccessPage : ContentPage
{
    private readonly AchievementService _achievementService;
    private bool _autoChecked = false;

    public SetupSuccessPage()
    {
        InitializeComponent();
        _achievementService = ServiceHelper.GetService<AchievementService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });

        if (!_autoChecked)
        {
            _autoChecked = true;
            try
            {
                var newlyUnlocked = await _achievementService.CheckAllAchievementsAsync();

                Debug.WriteLine($"Auto-checking achievements after setup. Found {newlyUnlocked.Count} newly unlocked");

                if (newlyUnlocked.Count > 0)
                {
                    foreach (var achievement in newlyUnlocked)
                    {
                        Debug.WriteLine($"Showing unlock popup for achievement: {achievement.Name}");
                        await Navigation.PushModalAsync(new AchievementUnlockedPopup(achievement));
                    }
                }

                if (Application.Current != null)
                {
                    Application.Current.MainPage = new AppShell();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Auto-check Navigation error: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                if (Application.Current != null)
                {
                    Application.Current.MainPage = new AppShell();
                }
            }
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        // Keep backward compatibility: call the same logic as OnAppearing.
        if (!_autoChecked)
        {
            OnAppearing();
        }
    }
}