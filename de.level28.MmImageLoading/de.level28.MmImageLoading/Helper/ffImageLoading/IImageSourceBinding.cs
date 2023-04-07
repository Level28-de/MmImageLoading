
namespace MmImageLoading.MAUI
{
    public interface IImageSourceBinding
    {
        MmImageLoading.Work.ImageSource ImageSource { get; }

        string Path { get; }

        Func<CancellationToken, Task<Stream>> Stream { get; }
    }
}
