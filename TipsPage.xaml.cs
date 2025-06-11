using Diploma_cs.SecondaryPages;

namespace Diploma_cs;

public partial class TipsPage : ContentPage
{
	public TipsPage()
	{
		InitializeComponent();
	}

    async void OnTileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ArticleViewPage());
    }

    private void OnTileClicked(object sender, TappedEventArgs e)
    {

    }
}