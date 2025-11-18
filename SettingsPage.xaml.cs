using Diploma_cs.SecondaryPages;

namespace Diploma_cs;

public partial class SettingsPage : ContentPage
{
	private int tapCount = 0;
	private DateTime lastTapTime = DateTime.MinValue;
	private const int TapsTarget = 5;
	private const int ResetCountdownMs = 1000;

    public SettingsPage()
	{
		InitializeComponent();
	}

	public async void OnLabelTapped(object sender, TappedEventArgs e)
	{
		if(DateTime.Now - lastTapTime > TimeSpan.FromMilliseconds(ResetCountdownMs))
		{
			tapCount = 0;
        }

		tapCount++;
		lastTapTime = DateTime.Now;

		if (tapCount == TapsTarget)
		{
			tapCount = 0;
			await Navigation.PushAsync(new DebugPage());
        }
    }
}