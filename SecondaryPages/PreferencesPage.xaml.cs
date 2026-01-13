namespace Diploma_cs.SecondaryPages;

public partial class PreferencesPage : ContentPage
{
    public PreferencesPage()
    {
        InitializeComponent();
    }

    private async void OnEditPersonalDataTapped(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new EditPersonalDataPage());

    private async void OnNotificationsTapped(object sender, TappedEventArgs e)
        => await Navigation.PushAsync(new NotificationPreferencesPage());
}