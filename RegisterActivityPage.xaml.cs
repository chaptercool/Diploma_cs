using System.Threading.Tasks;
using Diploma_cs.SecondaryPages;
using Diploma_cs.Data.Services;
using Diploma_cs.Services;

namespace Diploma_cs;

public partial class RegisterActivityPage : ContentPage
{
	private readonly AppDataService _appDataService;

	public RegisterActivityPage()
	{
		InitializeComponent();
		_appDataService = ServiceHelper.GetService<AppDataService>();
	}

	async void OnButtonClicked(object sender, EventArgs e)
	{
		try
		{
			await _appDataService.RegisterSmokingSessionAsync();

			await Navigation.PushAsync(new SavedMessagePage());
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