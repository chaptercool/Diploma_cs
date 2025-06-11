namespace Diploma_cs.SecondaryPages;

public partial class ReasonForActivityPage : ContentPage
{
	public ReasonForActivityPage()
	{
		InitializeComponent();
	}

    async void OnButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SavedMessagePage());
    }

    async void OnBackButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}