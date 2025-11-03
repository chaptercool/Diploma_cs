using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace Diploma_cs
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder.UseMauiApp<App>().ConfigureFonts(fonts =>
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
            }).UseMauiCommunityToolkit();
#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}