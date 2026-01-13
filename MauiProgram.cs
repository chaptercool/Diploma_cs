using CommunityToolkit.Maui;
using Diploma_cs.Data.Repositories;
using Diploma_cs.Data.Services;
using Diploma_cs.Data.Services.Achievements;
using Diploma_cs.Services.Notifications;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Plugin.LocalNotification;

namespace Diploma_cs
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseSkiaSharp()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Onest-Thin.ttf", "OnestThin");
                    fonts.AddFont("Onest-ExtraLight.ttf", "OnestExtraLight");
                    fonts.AddFont("Onest-Light.ttf", "OnestLight");
                    fonts.AddFont("Onest-Regular.ttf", "OnestRegular");
                    fonts.AddFont("Onest-Medium.ttf", "OnestMedium");
                    fonts.AddFont("Onest-SemiBold.ttf", "OnestSemiBold");
                    fonts.AddFont("Onest-Bold.ttf", "OnestBold");
                    fonts.AddFont("Onest-ExtraBold.ttf", "OnestExtraBold");
                    fonts.AddFont("Onest-Black.ttf", "OnestBlack");
                })
                .UseMauiCommunityToolkit()
                .UseLocalNotification();

            builder.Services.AddSingleton<AppNotificationSettings>();
            builder.Services.AddSingleton<IAppNotificationService, NotificationService>();

            builder.Services.AddSingleton<AppDataService>();
            builder.Services.AddSingleton<StatisticsCalculationService>();
            builder.Services.AddSingleton<UiStatisticsService>();
            builder.Services.AddSingleton<AchievementService>();

            builder.Services.AddSingleton<CsvUserProfileService>();
            builder.Services.AddSingleton<CsvSessionService>();
            builder.Services.AddSingleton<CsvDailyStatsService>();
            builder.Services.AddSingleton<CsvWeeklyStatsService>();
            builder.Services.AddSingleton<CsvTargetService>();

            builder.Services.AddSingleton<UserProfileRepository>();
            builder.Services.AddSingleton<TargetRepository>();
            builder.Services.AddSingleton<SessionRepository>();
            builder.Services.AddSingleton<DailyStatsRepository>();
            builder.Services.AddSingleton<WeeklyStatsRepository>();
            builder.Services.AddSingleton<AchievementRepository>();


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}