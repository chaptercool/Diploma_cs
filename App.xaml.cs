using Diploma_cs.Data.Services;
using Diploma_cs.Services;
using Diploma_cs.Setup;

namespace Diploma_cs
{
    public partial class App : Application
    {
        private bool _setupCheckCompleted = false;

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {
            base.OnStart();
            
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await PerformSetupCheckAsync();
            });
        }

        private async Task PerformSetupCheckAsync()
        {
            if (_setupCheckCompleted)
                return;

            _setupCheckCompleted = true;

            try
            {
                var appDataService = ServiceHelper.GetService<AppDataService>();
                await appDataService.InitializeAsync();

                if (!appDataService.IsSetupComplete())
                {
                    MainPage = new NavigationPage(new SetupStartPage());
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"App.PerformSetupCheck error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}