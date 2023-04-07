using MmImageLoading.Sample.MarkUp.Resources;

namespace MmImageLoading.Sample.MarkUp;


class App : Application
{
    public App(AppShell shell)
    {
        Resources = new AppStyles();

        MainPage = shell;
    }


}