using Microsoft.Maui.Handlers;

namespace MmImageLoading.MAUI
{
    using MmImageLoading.Forms.Platform.iOS;
#if IOS

    using MmImageLoading.Work;
    using UIKit;

    public partial class CachedImageHandler : ViewHandler<ICachedImage, UIImageView>
    {
        private readonly object _updateBitmapLock = new object();
        private bool _isDisposed;
        private ImageSourceBinding _lastImageSource;
        private IScheduledWork _currentTask;

        private static void MapAspect(CachedImageHandler arg1, ICachedImage arg2)
        {
            arg1.UpdateAspect();
        }

        private static void MapSource(CachedImageHandler arg1, ICachedImage arg2)
        {
            arg1.UpdateImage(arg1.NativeView, (CachedImage)arg2, null);
        }

        public UIImageView NativeView { get; private set; }

        protected override UIImageView CreatePlatformView() => new UIImageView();

        protected override void ConnectHandler(UIImageView platformView)
        {
            NativeView = platformView;

            VirtualView.InternalReloadImage = new Action(ReloadImage);
            VirtualView.InternalCancel = new Action(CancelIfNeeded);
            VirtualView.InternalGetImageAsJPG = new Func<GetImageAsJpgArgs, Task<byte[]>>(GetImageAsJpgAsync);
            VirtualView.InternalGetImageAsPNG = new Func<GetImageAsPngArgs, Task<byte[]>>(GetImageAsPngAsync);

            UpdateImage(NativeView, (CachedImage)VirtualView, null);
            UpdateAspect();

            base.ConnectHandler(platformView);
        }



        protected override void DisconnectHandler(UIImageView platformView)
        {
            VirtualView.InternalReloadImage = null;
            VirtualView.InternalCancel = null;
            VirtualView.InternalGetImageAsJPG = null;
            VirtualView.InternalGetImageAsPNG = null;

            base.DisconnectHandler(platformView);
        }


        private void ReloadImage()
        {
            UpdateImage(NativeView, (CachedImage)VirtualView, null);
        }

        private void CancelIfNeeded()
        {
            try
            {
                _currentTask?.Cancel();
            }
            catch { }
        }

        private Task<byte[]> GetImageAsJpgAsync(GetImageAsJpgArgs args)
        {
            return GetImageAsByteAsync(false, args.Quality, args.DesiredWidth, args.DesiredHeight);
        }

        private Task<byte[]> GetImageAsPngAsync(GetImageAsPngArgs args)
        {
            return GetImageAsByteAsync(true, 90, args.DesiredWidth, args.DesiredHeight);
        }

        private async Task<byte[]> GetImageAsByteAsync(bool usePNG, int quality, int desiredWidth, int desiredHeight)
        {
            UIImage image = null;

            await ImageService.Instance.Config.MainThreadDispatcher.PostAsync(() =>
            {
                if (NativeView != null)
                    image = NativeView.Image;
            }).ConfigureAwait(false);

            if (image == null)
                return null;

            if (desiredWidth != 0 || desiredHeight != 0)
            {
                //TODO needs to be done
                //image = image.ResizeUIImage(desiredWidth, desiredHeight, InterpolationMode.Default);
            }

#if __IOS__

            var imageData = usePNG ? image.AsPNG() : image.AsJPEG((nfloat)quality / 100f);

            if (imageData == null || imageData.Length == 0)
                return null;

            var encoded = imageData.ToArray();
            imageData.TryDispose();
            return encoded;
#elif __MACOS__

            byte[] encoded;
            using (MemoryStream ms = new MemoryStream())
            using (var stream = usePNG ? image.AsPngStream() : image.AsJpegStream(quality))
            {
                stream.CopyTo(ms);
                encoded = ms.ToArray();
            }

            if (desiredWidth != 0 || desiredHeight != 0)
            {
                image.TryDispose();
            }

            return encoded;
#endif
        }

        private void UpdateImage(UIImageView imageView, CachedImage image, CachedImage previousImage)
        {
            lock (_updateBitmapLock)
            {
                CancelIfNeeded();

                if (image == null || imageView == null || imageView.Handle == IntPtr.Zero || _isDisposed)
                    return;

                var ffSource = ImageSourceBinding.GetImageSourceBinding(image.Source, image);
                if (ffSource == null)
                {
                    if (_lastImageSource == null)
                        return;

                    _lastImageSource = null;
                    imageView.Image = null;
                    return;
                }

                if (previousImage != null && !ffSource.Equals(_lastImageSource))
                {
                    _lastImageSource = null;
                    imageView.Image = null;
                }

                image.IsLoading = true;

                var placeholderSource = ImageSourceBinding.GetImageSourceBinding(image.LoadingPlaceholder, image);
                var errorPlaceholderSource = ImageSourceBinding.GetImageSourceBinding(image.ErrorPlaceholder, image);
                image.SetupOnBeforeImageLoading(out var imageLoader, ffSource, placeholderSource, errorPlaceholderSource);

                if (imageLoader != null)
                {
                    var finishAction = imageLoader.OnFinish;
                    var sucessAction = imageLoader.OnSuccess;

                    imageLoader.Finish((work) =>
                    {
                        finishAction?.Invoke(work);
                        ImageLoadingSizeChanged(image, false);
                    });

                    imageLoader.Success((imageInformation, loadingResult) =>
                    {
                        sucessAction?.Invoke(imageInformation, loadingResult);
                        _lastImageSource = ffSource;
                    });

                    imageLoader.LoadingPlaceholderSet(() => ImageLoadingSizeChanged(image, true));

                    if (!_isDisposed)
                        _currentTask = imageLoader.Into(imageView);
                }
            }
        }

        private async void ImageLoadingSizeChanged(CachedImage element, bool isLoading)
        {
            if (element == null || _isDisposed)
                return;

            await MmImageLoading.ImageService.Instance.Config.MainThreadDispatcher.PostAsync(() =>
            {
                if (element == null || _isDisposed)
                    return;

                ((IVisualElementController)element).PlatformSizeChanged();

                if (!isLoading)
                    element.IsLoading = isLoading;
            }).ConfigureAwait(false);
        }

        public void UpdateAspect()
        {

            if (VirtualView.Aspect == Aspect.AspectFill)
                NativeView.ContentMode = UIViewContentMode.ScaleAspectFill;

            else if (VirtualView.Aspect == Aspect.Fill)
                NativeView.ContentMode = UIViewContentMode.ScaleToFill;

            else
                NativeView.ContentMode = UIViewContentMode.ScaleAspectFit;
        }



    }

#endif
}
