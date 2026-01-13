using System;
using System.Globalization;
using Microsoft.Maui.Controls;
using Diploma_cs.Data.Services;
using Diploma_cs.Services;

namespace Diploma_cs.Setup;

public partial class SetupGeneralInfoPage : ContentPage
{
    private readonly AppDataService _appDataService;

    public SetupGeneralInfoPage()
    {
        InitializeComponent();
        _appDataService = ServiceHelper.GetService<AppDataService>();
    }

    private async void OnNextButtonClicked(object sender, EventArgs e)
    {
        string name = UserName?.Text?.Trim() ?? string.Empty;
        string ageText = UserAge?.Text?.Trim() ?? string.Empty;
        object gender = UserGender?.SelectedItem;

        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(ageText) || gender == null)
        {
            await DisplayAlert("Brak danych", "Proszê wype³niæ wszystkie pola.", "OK");
            return;
        }

        if (name.Length > 100)
        {
            await DisplayAlert("B³¹d", "Zbyt d³ugie imiê", "OK");
            return;
        }

        if (!int.TryParse(ageText, NumberStyles.Integer, CultureInfo.InvariantCulture, out int age))
        {
            await DisplayAlert("B³¹d", "Wiek musi byæ liczb¹ ca³kowit¹.", "OK");
            return;
        }

        if (age < 0 || age > 99)
        {
            await DisplayAlert("B³¹d", "WprowadŸ poprawny wiek.", "OK");
            return;
        }

        if (age < 18)
        {
            await DisplayAlert("B³¹d", "Musisz mieæ co najmniej 18 lat, aby korzystaæ z aplikacji.", "OK");
            Environment.Exit(0);
            return;
        }

        try
        {
            var setupSession = new SetupSessionData
            {
                Name = name,
                Age = age,
                Gender = gender.ToString() ?? string.Empty
            };

            await Navigation.PushAsync(new SetupTargetsPage { SetupSession = setupSession });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Unknown error: {ex.Message}", "OK");
        }
    }
}