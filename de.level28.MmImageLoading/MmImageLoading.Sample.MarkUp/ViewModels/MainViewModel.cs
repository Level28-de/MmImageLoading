namespace MmImageLoading.Sample.MarkUp.ViewModels;

sealed partial class MainViewModel : BaseViewModel, IDisposable
{
    readonly IDispatcher dispatcher;

    [RelayCommand]
    public void LocalGifImageButton() => NavigateLocalGifPage();

    [RelayCommand]
    public void OnlineGifImageButton() => NavigateOnlineGifPage();

    public MainViewModel(IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }

    Task NavigateLocalGifPage() => dispatcher.DispatchAsync(() =>
    {
        var route = AppShell.GetRoute<LocalPage, LocalViewModel>();
        return Shell.Current.GoToAsync(route);
    });

    Task NavigateOnlineGifPage() => dispatcher.DispatchAsync(() =>
    {
        var route = AppShell.GetRoute<OnlinePage, OnlineViewModel>();
        return Shell.Current.GoToAsync(route);
    });

    public void Dispose()
    {
    }

}