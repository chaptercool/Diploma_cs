namespace Diploma_cs.Setup;

public partial class SetupStartPage : ContentPage
{
    public SetupStartPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SetupGeneralInfoPage());
    }
}