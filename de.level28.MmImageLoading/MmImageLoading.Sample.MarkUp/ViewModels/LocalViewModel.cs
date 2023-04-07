namespace MmImageLoading.Sample.MarkUp.ViewModels;

sealed partial class LocalViewModel : BaseViewModel, IDisposable
{
    readonly IDispatcher dispatcher;


    public LocalViewModel(IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
    }


    public void Dispose()
    {
    }




}