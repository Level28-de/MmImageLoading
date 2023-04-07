namespace MmImageLoading.Sample.MarkUp.ViewModels;

sealed partial class OnlineViewModel : BaseViewModel, IDisposable
{
    readonly IDispatcher dispatcher;


    public OnlineViewModel(IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
        GifAdress1 = "https://eltepi.de/images/AcceleratedContentCreation.gif";
        GifAdress2 = "https://eltepi.de/images/customizedVocabulary.gif";
        GifAdress3 = "https://eltepi.de/images/DistractionFree.gif";
        GifAdress4 = "https://eltepi.de/images/LongtermMemory.gif";
        GifAdress5 = "https://elt.de/images/LongtermMemory.gif";
        LoadingPlaceholder = "logo";
        ErrorPlaceholder = "image_missing";
    }

    [ObservableProperty]
    string gifAdress2;

    [ObservableProperty]
    string gifAdress3;

    [ObservableProperty]
    string gifAdress4;

    [ObservableProperty]
    string gifAdress5;

    [ObservableProperty]
    string loadingPlaceholder;

    [ObservableProperty]
    string errorPlaceholder;

    string myGifAdress1;
    public string GifAdress1
    {
        get
        {
            return myGifAdress1;
        }
        set
        {
            myGifAdress1 = value;
            OnPropertyChanged(nameof(GifAdress1));
        }
    }
    public void Dispose()
    {
    }




}