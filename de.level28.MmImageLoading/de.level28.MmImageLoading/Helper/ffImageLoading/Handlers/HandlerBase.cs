using System;
using System.Threading;
using System.Threading.Tasks;
using MmImageLoading.Work;


namespace MmImageLoading.MAUI.Handlers
{
    public abstract class HandlerBase<TNativeView>
    {

        protected virtual Task<IImageLoaderTask> LoadImageAsync(IImageSourceBinding binding, Microsoft.Maui.Controls.ImageSource imageSource, TNativeView imageView, CancellationToken cancellationToken)
        {
#if (ANDROID || IOS)
            TaskParameter parameters = default;

            if (binding.ImageSource == MmImageLoading.Work.ImageSource.Url)
            {
                var urlSource = (Microsoft.Maui.Controls.UriImageSource)((imageSource as IVectorImageSource)?.ImageSource ?? imageSource);
                parameters = MmImageLoading.ImageService.Instance.LoadUrl(binding.Path, urlSource.CacheValidity);

                if (!urlSource.CachingEnabled)
                {
                    parameters.WithCache(Cache.CacheType.None);
                }
            }
            else if (binding.ImageSource == MmImageLoading.Work.ImageSource.CompiledResource)
            {
                parameters = MmImageLoading.ImageService.Instance.LoadCompiledResource(binding.Path);
            }
            else if (binding.ImageSource == MmImageLoading.Work.ImageSource.ApplicationBundle)
            {
                parameters = MmImageLoading.ImageService.Instance.LoadFileFromApplicationBundle(binding.Path);
            }
            else if (binding.ImageSource == MmImageLoading.Work.ImageSource.Filepath)
            {
                parameters = MmImageLoading.ImageService.Instance.LoadFile(binding.Path);
            }
            else if (binding.ImageSource == MmImageLoading.Work.ImageSource.Stream)
            {
                parameters = MmImageLoading.ImageService.Instance.LoadStream(binding.Stream);
            }
            else if (binding.ImageSource == MmImageLoading.Work.ImageSource.EmbeddedResource)
            {
                parameters = MmImageLoading.ImageService.Instance.LoadEmbeddedResource(binding.Path);
            }

            if (parameters != default)
            {
                // Enable vector image source
                if (imageSource is IVectorImageSource vect)
                {
                    parameters.WithCustomDataResolver(vect.GetVectorDataResolver());
                }

                var tcs = new TaskCompletionSource<IImageLoaderTask>();

                parameters
                    .FadeAnimation(false, false)
                    .Error(ex =>
                    {
                        tcs.TrySetException(ex);
                    })
                    .Finish(scheduledWork =>
                    {
                        tcs.TrySetResult(scheduledWork as IImageLoaderTask);
                    });

                if (cancellationToken.IsCancellationRequested)
                    return Task.FromResult<IImageLoaderTask>(null);

                var task = GetImageLoaderTask(parameters, imageView);

                if (cancellationToken != default)
                    cancellationToken.Register(() =>
                    {
                        try
                        {
                            task?.Cancel();
                        }
                        catch { }
                    });

                return tcs.Task;
            }

            return Task.FromResult<IImageLoaderTask>(null);
#else
            return null;
#endif
        }
        protected abstract IImageLoaderTask GetImageLoaderTask(TaskParameter parameters, TNativeView imageView);
    }
}
