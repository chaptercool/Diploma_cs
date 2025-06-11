namespace Diploma_cs.SecondaryPages;

public partial class SavedMessagePage : ContentPage
{
    public SavedMessagePage()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(3000);
        if (Navigation.NavigationStack.Count > 1)
        {
            await Navigation.PopToRootAsync();
        }
    }
}