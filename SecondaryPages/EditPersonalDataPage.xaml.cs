using System.Globalization;
using Diploma_cs.Data.Services;
using Diploma_cs.Services;

namespace Diploma_cs.SecondaryPages;

public partial class EditPersonalDataPage : ContentPage
{
    private readonly AppDataService _appDataService;

    public EditPersonalDataPage()
    {
        InitializeComponent();
        _appDataService = ServiceHelper.GetService<AppDataService>();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            var profile = await _appDataService.GetUserProfileAsync();
            if (profile == null)
                return;

            UserNameEntry.Text = profile.Name;
            UserAgeEntry.Text = profile.Age.ToString(CultureInfo.InvariantCulture);

            if (!string.IsNullOrWhiteSpace(profile.Gender))
            {
                var idx = UserGenderPicker.Items.IndexOf(profile.Gender);
                if (idx >= 0)
                    UserGenderPicker.SelectedIndex = idx;
            }
        }
        catch
        {
            // keep silent; page still usable
        }
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        var name = UserNameEntry?.Text?.Trim() ?? string.Empty;
        var ageText = UserAgeEntry?.Text?.Trim() ?? string.Empty;
        var gender = UserGenderPicker?.SelectedItem?.ToString() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(ageText) || string.IsNullOrWhiteSpace(gender))
        {
            await DisplayAlert("Brak danych", "Proszê wype³niæ wszystkie pola.", "OK");
            return;
        }

        if (name.Length > 100)
        {
            await DisplayAlert("B³¹d", "Zbyt d³ugie imiê.", "OK");
            return;
        }

        if (!int.TryParse(ageText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var age))
        {
            await DisplayAlert("B³¹d", "Wiek musi byæ liczb¹ ca³kowit¹.", "OK");
            return;
        }

        if (age < 18 || age > 99)
        {
            await DisplayAlert("B³¹d", "WprowadŸ poprawny wiek (18-99).", "OK");
            return;
        }

        try
        {
            var existing = await _appDataService.GetUserProfileAsync();
            if (existing == null)
            {
                await DisplayAlert("B³¹d", "Nie znaleziono profilu u¿ytkownika.", "OK");
                return;
            }

            // Preserve non-personal fields (targets/pricing) while updating personal data.
            await _appDataService.CompleteSetupAsync(
                name,
                age,
                gender,
                existing.DailyConsumption,
                existing.PackPrice);

            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("B³¹d", $"Nie uda³o siê zapisaæ danych: {ex.Message}", "OK");
        }
    }
}
