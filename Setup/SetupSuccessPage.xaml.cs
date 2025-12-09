using System;
using Microsoft.Maui.Controls;
using System.Diagnostics;
using Diploma_cs.Services;
using Diploma_cs.Data.Services;

namespace Diploma_cs.Setup;

public partial class SetupSuccessPage : ContentPage
{
    public SetupSuccessPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        Shell.SetBackButtonBehavior(this, new BackButtonBehavior { IsVisible = false });
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Navigation error: {ex.Message}");
            if (Application.Current != null)
            {
                Application.Current.MainPage = new AppShell();
            }
        }
    }
}