using de.level28.MmImageLoading;
using Microsoft.Extensions.Logging;


namespace MmImageLoading.Sample.MarkUp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()

#if (ANDROID || IOS)
                          .UseMmImageLoading()
#endif
                          .UseMauiCommunityToolkit()
                          .UseMauiCommunityToolkitMarkup()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        // App Shell
        builder.Services.AddTransient<AppShell>();

        builder.Services.AddSingleton<App>();
        builder.Services.AddSingleton(Browser.Default);
        builder.Services.AddSingleton(Preferences.Default);
        // Pages + View Models
        builder.Services.AddTransient<MainPage, MainViewModel>();
        builder.Services.AddTransient<LocalPage, LocalViewModel>();
        builder.Services.AddTransient<OnlinePage, OnlineViewModel>();

        return builder.Build();
    }
}

