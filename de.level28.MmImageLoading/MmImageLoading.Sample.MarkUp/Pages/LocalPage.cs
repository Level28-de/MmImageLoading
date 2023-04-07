
using MmImageLoading.MAUI;
using Microsoft.Maui.Layouts;

namespace MmImageLoading.Sample.MarkUp.Pages;

sealed class LocalPage : BaseContentPage<LocalViewModel>
{
    readonly IDispatcher dispatcher;
    private CachedImage MmCachedImage;
    private CachedImage MmCachedImage2;
    private CachedImage MmCachedImage3;
    private CachedImage MmCachedImage4;
    private CachedImage MmLoadingError;

    public LocalPage(IDispatcher dispatcher,
                    LocalViewModel localViewModel) : base(localViewModel, "local image")
    {
        this.dispatcher = dispatcher;

        ToolbarItems.Add(new ToolbarItem { Command = new RelayCommand(Animate) }.Text("fix"));

        MmCachedImage = new CachedImage()
        {
            Source = "eltepi.gif",
            LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
        };
        MmCachedImage2 = new CachedImage()
        {
            Source = "unypan.gif",
            LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
            WidthRequest = 100,
            HeightRequest = 100,

        };
        MmCachedImage3 = new CachedImage()
        {
            Source = "eltepi_second.gif",
            LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
            WidthRequest = 100,
            HeightRequest = 100,
            DownsampleToViewSize = true,

        };
        MmCachedImage4 = new CachedImage()
        {
            Source = "unypan_second.gif",
            LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
            CacheDuration = TimeSpan.FromDays(1),
            WidthRequest = 100,
            HeightRequest = 200,
            DownsampleToViewSize = true,
            BitmapOptimizations = false,
        };
        MmLoadingError = new CachedImage()
        {
            Source = "unypan_third.gif",
            LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
            WidthRequest = 100,
            HeightRequest = 200,
            CacheDuration = TimeSpan.FromDays(1),
            DownsampleToViewSize = true,
            RetryCount = 3,
            RetryDelay = 500,
        };
        Content = new VerticalStackLayout
        {
            Children = {
                MmCachedImage,
                MmCachedImage2,
                MmCachedImage3,
                MmCachedImage4,
                MmLoadingError
            }
        };
    }

    private void Animate()
    {
        MmLoadingError.Source = "logo";
        MmLoadingError.ReloadImage();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();


    }


    //Task NavigateToSettingsPage() => dispatcher.DispatchAsync(() =>
    //{
    //    var route = AppShell.GetRoute<SettingsPage, SettingsViewModel>();
    //    return Shell.Current.GoToAsync(route);
    //});


}

