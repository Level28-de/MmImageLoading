using MmImageLoading.MAUI.Platforms.Android;
using Microsoft.Maui.Handlers;

namespace MmImageLoading.MAUI
{
#if ANDROID

    using Android.Graphics;
    using Android.Graphics.Drawables;
    using MmImageLoading.Work;
    using static Android.Widget.ImageView;


    public partial class CachedImageHandler : ViewHandler<ICachedImage, CachedImageView>
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
            arg1.UpdateBitmap(arg1.NativeView, (CachedImage)arg2);
        }

        public CachedImageView NativeView { get; private set; }

        protected override CachedImageView CreatePlatformView() => new CachedImageView(Context);

        protected override void ConnectHandler(CachedImageView platformView)
        {
            NativeView = platformView;

            VirtualView.InternalReloadImage = new Action(ReloadImage);
            VirtualView.InternalCancel = new Action(CancelIfNeeded);
            VirtualView.InternalGetImageAsJPG = new Func<GetImageAsJpgArgs, Task<byte[]>>(GetImageAsJpgAsync);
            VirtualView.InternalGetImageAsPNG = new Func<GetImageAsPngArgs, Task<byte[]>>(GetImageAsPngAsync);

            UpdateBitmap(NativeView, (CachedImage)VirtualView);
            UpdateAspect();

            base.ConnectHandler(platformView);
        }



        protected override void DisconnectHandler(CachedImageView platformView)
        {
            VirtualView.InternalReloadImage = null;
            VirtualView.InternalCancel = null;
            VirtualView.InternalGetImageAsJPG = null;
            VirtualView.InternalGetImageAsPNG = null;

            base.DisconnectHandler(platformView);
        }


        private void ReloadImage()
        {
            UpdateBitmap(NativeView, (CachedImage)VirtualView);
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
            return GetImageAsByteAsync(Bitmap.CompressFormat.Jpeg, args.Quality, args.DesiredWidth, args.DesiredHeight);
        }

        private Task<byte[]> GetImageAsPngAsync(GetImageAsPngArgs args)
        {
            return GetImageAsByteAsync(Bitmap.CompressFormat.Png, 90, args.DesiredWidth, args.DesiredHeight);
        }

        private void UpdateBitmap(CachedImageView imageView, CachedImage image)
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
                    imageView.SetImageResource(global::Android.Resource.Color.Transparent);
                    return;
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
                        _currentTask = MmImageLoading.TaskParameterPlatformExtensions.Into(imageLoader, imageView);
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
                NativeView.SetScaleType(ScaleType.CenterCrop);

            else if (VirtualView.Aspect == Aspect.Fill)
                NativeView.SetScaleType(ScaleType.FitXy);

            else
                NativeView.SetScaleType(ScaleType.FitCenter);
        }

        private async Task<byte[]> GetImageAsByteAsync(Bitmap.CompressFormat format, int quality, int desiredWidth, int desiredHeight)
        {
            if (!(NativeView.Drawable is BitmapDrawable drawable) || drawable.Bitmap == null)
                return null;

            var bitmap = drawable.Bitmap;

            if (desiredWidth != 0 || desiredHeight != 0)
            {
                var widthRatio = (double)desiredWidth / bitmap.Width;
                var heightRatio = (double)desiredHeight / bitmap.Height;
                var scaleRatio = Math.Min(widthRatio, heightRatio);

                if (desiredWidth == 0)
                    scaleRatio = heightRatio;

                if (desiredHeight == 0)
                    scaleRatio = widthRatio;

                var aspectWidth = (int)(bitmap.Width * scaleRatio);
                var aspectHeight = (int)(bitmap.Height * scaleRatio);

                bitmap = Bitmap.CreateScaledBitmap(bitmap, aspectWidth, aspectHeight, true);
            }

            using (var stream = new MemoryStream())
            {
                await bitmap.CompressAsync(format, quality, stream).ConfigureAwait(false);
                var compressed = stream.ToArray();

                if (desiredWidth != 0 || desiredHeight != 0)
                {
                    bitmap.Recycle();
                    bitmap.TryDispose();
                }

                return compressed;
            }
        }

    }

#endif
}
