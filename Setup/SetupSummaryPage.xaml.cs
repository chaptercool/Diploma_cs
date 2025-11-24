namespace Diploma_cs.Setup;

public partial class SetupSummaryPage : ContentPage
{
	public SetupSummaryPage()
	{
		InitializeComponent();
	}

    private async void OnNextButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SetupSuccessPage());
    }
}