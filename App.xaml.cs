using Diploma_cs.Data.Services;
using Diploma_cs.Services;
using Diploma_cs.Services.Notifications;
using Diploma_cs.Setup;
using System.Text;

namespace Diploma_cs
{
    public partial class App : Application
    {
        private bool _setupCheckCompleted = false;

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new AppShell());
        }

        protected override void OnStart()
        {
            base.OnStart();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await CopyEmbeddedFilesAsync();
                await PerformSetupCheckAsync();
                await InitializeNotificationsAsync();
            });
        }

        private async Task InitializeNotificationsAsync()
        {
            try
            {
                var notificationService = ServiceHelper.TryGetService<IAppNotificationService>();
                if (notificationService == null)
                    return;

                await notificationService.RescheduleDailyRemindersAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"App.InitializeNotificationsAsync error: {ex.Message}");
            }
        }

        private async Task CopyEmbeddedFilesAsync()
        {
            try
            {
                var achievementsFilePath = Path.Combine(FileSystem.AppDataDirectory, "achievements.csv");
                if (!File.Exists(achievementsFilePath))
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync("Data/Misc/achievements.csv");
                    using var ms = new MemoryStream();
                    await stream.CopyToAsync(ms);
                    var bytes = ms.ToArray();

                    string content;
                    content = Encoding.UTF8.GetString(bytes);
                    bool hasReplacement = content.IndexOf('\uFFFD') >= 0;

                    if (hasReplacement)
                    {
                        try
                        {
                            var cp1250 = Encoding.GetEncoding(1250);
                            content = cp1250.GetString(bytes);
                        }
                        catch
                        {
                        }
                    }

                    await File.WriteAllTextAsync(achievementsFilePath, content, Encoding.UTF8);
                    System.Diagnostics.Debug.WriteLine("achievements.csv copied successfully (encoding normalized)");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error copying achievements.csv: {ex.Message}");
            }
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