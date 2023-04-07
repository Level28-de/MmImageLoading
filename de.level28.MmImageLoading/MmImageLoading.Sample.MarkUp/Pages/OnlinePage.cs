
using MmImageLoading.MAUI;
using Microsoft.Maui.Layouts;

namespace MmImageLoading.Sample.MarkUp.Pages;

sealed class OnlinePage : BaseContentPage<OnlineViewModel>
{
    readonly IDispatcher dispatcher;
    private CachedImage MmCachedImage;
    private CachedImage MmCachedImage2;
    private CachedImage MmCachedImage3;
    private CachedImage MmCachedImage4;
    private CachedImage MmLoadingError;

    public OnlinePage(IDispatcher dispatcher,
                    OnlineViewModel onlineViewModel) : base(onlineViewModel, "Online image")
    {
        this.dispatcher = dispatcher;

        ToolbarItems.Add(new ToolbarItem { Command = new RelayCommand(Animate) }.Text("fix"));

        MmCachedImage = new CachedImage()
        {
            //Source = "eltepi.gif",
            LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
        }.Bind(CachedImage.SourceProperty, static (OnlineViewModel vm) => vm.GifAdress1,
                                mode: BindingMode.OneTime)
        ;
        MmCachedImage2 = new CachedImage()
        {
            LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
            WidthRequest = 100,
            HeightRequest = 100,

        }.Bind(CachedImage.SourceProperty, static (OnlineViewModel vm) => vm.GifAdress2,
                                mode: BindingMode.OneTime);
        MmCachedImage3 = new CachedImage()
        {
            //LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
            WidthRequest = 100,
            HeightRequest = 100,
            DownsampleToViewSize = true,

        }.Bind(CachedImage.SourceProperty, static (OnlineViewModel vm) => vm.GifAdress3,
                                mode: BindingMode.OneWay)
        .Bind(CachedImage.LoadingPlaceholderProperty, static (OnlineViewModel vm) => vm.LoadingPlaceholder,
                                mode: BindingMode.OneWay)

        ;
        MmCachedImage4 = new CachedImage()
        {
            LoadingPlaceholder = "logo",
            ErrorPlaceholder = "image_missing",
            CacheDuration = TimeSpan.FromDays(1),
            WidthRequest = 100,
            HeightRequest = 200,
            DownsampleToViewSize = true,

        }.Bind(CachedImage.SourceProperty, static (OnlineViewModel vm) => vm.GifAdress4,
                                mode: BindingMode.OneWay);
        MmLoadingError = new CachedImage()
        {
            LoadingPlaceholder = "logo",
            //ErrorPlaceholder = "image_missing",
            WidthRequest = 100,
            HeightRequest = 200,
            CacheDuration = TimeSpan.FromDays(1),
            DownsampleToViewSize = true,
            RetryCount = 3,
            RetryDelay = 500,
        }.Bind(CachedImage.SourceProperty, static (OnlineViewModel vm) => vm.GifAdress5,
                                mode: BindingMode.OneWay)
          .Bind(CachedImage.ErrorPlaceholderProperty, static (OnlineViewModel vm) => vm.ErrorPlaceholder,
                                mode: BindingMode.OneWay);
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
        var viewModel = (OnlineViewModel)BindingContext;

        viewModel.GifAdress5 = viewModel.GifAdress3;

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

