using MmImageLoading.MAUI;
using Microsoft.Maui.Layouts;

namespace MmImageLoading.Sample.MarkUp.Pages;

sealed class MainPage : BaseContentPage<MainViewModel>
{
    readonly IDispatcher dispatcher;

    public MainPage(IDispatcher dispatcher,
                    MainViewModel mainViewModel) : base(mainViewModel, "MmImageLoading")
    {
        this.dispatcher = dispatcher;

        var localGifButton = new Button()
                        .Text("local gifs")
                        .Font(bold: true)
                        .CenterHorizontal()
                        .BindCommand(
                               static (MainViewModel vm) => vm.LocalGifImageButtonCommand, mode: BindingMode.OneWay);

        var onlineGifButton = new Button()
                 .Text("online gifs")
                 .Font(bold: true)
                 .CenterHorizontal()
                 .BindCommand(
                        static (MainViewModel vm) => vm.OnlineGifImageButtonCommand, mode: BindingMode.OneWay);


        Content = new VerticalStackLayout
        {
            Children = {
                localGifButton,
                onlineGifButton
            }
        };
    }


    protected override void OnAppearing()
    {
        base.OnAppearing();


    }

}