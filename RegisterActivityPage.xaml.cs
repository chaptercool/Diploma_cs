using System.Threading.Tasks;
using Diploma_cs.SecondaryPages;
using Diploma_cs.Data.Services;
using Diploma_cs.Data.Services.Achievements;
using Diploma_cs.Services;
using Diploma_cs.Popups;

namespace Diploma_cs;

public partial class RegisterActivityPage : ContentPage
{
	private readonly AppDataService _appDataService;
	private readonly AchievementService _achievementService;

	public RegisterActivityPage()
	{
		InitializeComponent();
		_appDataService = ServiceHelper.GetService<AppDataService>();
		_achievementService = ServiceHelper.GetService<AchievementService>();
	}

	async void OnButtonClicked(object sender, EventArgs e)
	{
		try
		{
			await _appDataService.RegisterSmokingSessionAsync();

			await Navigation.PushAsync(new SavedMessagePage());

			var newlyUnlocked = await _achievementService.CheckAllAchievementsAsync();

			if (newlyUnlocked.Count > 0)
			{
				foreach (var achievement in newlyUnlocked)
				{
					await Navigation.PushModalAsync(new AchievementUnlockedPopup(achievement));
				}
			}
		}
		catch (Exception ex)
		{
			await DisplayAlert("B³¹d", $"Nie uda³o siê zarejestrowaæ aktywnoœci: {ex.Message}", "OK");
		}
    }

	async void OnPackPurchaseClicked(object sender, EventArgs e)
	{
		try
		{
			await _appDataService.RegisterPackPurchaseAsync();

			await Navigation.PushAsync(new SavedMessagePage());
		}
		catch (Exception ex)
		{
			await DisplayAlert("B³¹d", $"Nie uda³o siê zarejestrowaæ aktywnoœci: {ex.Message}", "OK");
		}
	}
}