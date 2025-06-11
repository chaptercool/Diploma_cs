using System.Threading.Tasks;
using Diploma_cs.SecondaryPages;

namespace Diploma_cs;

public partial class RegisterActivityPage : ContentPage
{
	public RegisterActivityPage()
	{
		InitializeComponent();
	}

	async void OnButtonClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new ReasonForActivityPage());
		//DisplayAlert("Button Clicked", "You clicked the button!", "OK");
    }
}