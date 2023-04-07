using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using MmImageLoading.Cache;
using System.Reflection;
using System.ComponentModel;
using MmImageLoading.MAUI;

namespace MmImageLoading.MAUI
{


    /// <summary>
    /// CachedImage by Daniel Luberda
    /// </summary>
    public class CachedImage : View, ICachedImage
    {
        private ImageSource _loadingPlaceholder;
        private ImageSource _errorPlaceholder;
        private ImageSource _source = default(ImageSource);



        #region ICachedImage

        public Aspect Aspect { get; set; } = Aspect.AspectFit;

        public bool IsLoadingKey { get; set; }
        public bool IsLoading { get; set; }

        public bool IsOpaque { get; set; }

        //[TypeConverter(typeof(ImageSourceConverter))]

        public static readonly BindableProperty SourceProperty = BindableProperty.Create(nameof(Source), typeof(ImageSource), typeof(CachedImage), default(ImageSource), BindingMode.OneWay, coerceValue: CoerceImageSource, propertyChanged: OnSourcePropertyChanged);

        private static object CoerceImageSource(BindableObject bindable, object newValue)
        {
            return ((CachedImage)bindable).CoerceImageSource(newValue);
        }
        [TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource Source
        {
            get => (ImageSource)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }
        public int RetryCount { get; set; } = 3;

        public int RetryDelay { get; set; } = 250;

        public int? LoadingDelay { get; set; }

        public double DownsampleWidth { get; set; } = 0d;

        public double DownsampleHeight { get; set; } = 0d;

        public bool DownsampleToViewSize { get; set; } = false;

        public bool DownsampleUseDipUnits { get; set; }

        public TimeSpan? CacheDuration { get; set; }

        public Work.LoadingPriority LoadingPriority { get; set; } = Work.LoadingPriority.Normal;

        public bool? BitmapOptimizations { get; set; }

        public bool? FadeAnimationEnabled { get; set; }

        public bool? FadeAnimationForCachedImages { get; set; }

        public int? FadeAnimationDuration { get; set; }

        //[TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource LoadingPlaceholder
        {
            get => CoerceImageSource(_loadingPlaceholder);
            set => _loadingPlaceholder = value;
        }

        //[TypeConverter(typeof(ImageSourceConverter))]
        public ImageSource ErrorPlaceholder
        {
            get => CoerceImageSource(_errorPlaceholder);
            set => _errorPlaceholder = value;
        }

        public bool? TransformPlaceholders { get; set; }

        public bool? InvalidateLayoutAfterLoaded { get; set; }

        public ICommand FileWriteFinishedCommand { get; set; }

        #endregion

        protected virtual ImageSource CoerceImageSource(object newValue)
        {
            var uriImageSource = newValue as UriImageSource;

            if (uriImageSource?.Uri?.OriginalString != null)
            {
                if (uriImageSource.Uri.Scheme.Equals("file", StringComparison.OrdinalIgnoreCase))
                    return ImageSource.FromFile(uriImageSource.Uri.LocalPath);

                if (uriImageSource.Uri.Scheme.Equals("resource", StringComparison.OrdinalIgnoreCase))
                    return new EmbeddedResourceImageSource(uriImageSource.Uri);

                if (uriImageSource.Uri.OriginalString.IsDataUrl())
                    return new DataUrlImageSource(uriImageSource.Uri.OriginalString);
            }

            return newValue as ImageSource;
        }



        private static bool? _isDesignModeEnabled = null;
        protected static bool IsDesignModeEnabled
        {
            get
            {
                // works only on Xamarin.Forms >= 3.0
                if (!_isDesignModeEnabled.HasValue)
                {
                    var type = typeof(Image).GetTypeInfo().Assembly.GetType("Xamarin.Forms.DesignMode");
                    if (type == null)
                    {
                        _isDesignModeEnabled = true;
                    }
                    else
                    {
                        var property = type.GetRuntimeProperty("IsDesignModeEnabled");
                        _isDesignModeEnabled = (bool)property.GetValue(null);
                    }
                }

                return _isDesignModeEnabled.Value;
            }
        }

        private static readonly PropertyInfo _visualMarkerProperty = typeof(VisualElement).GetTypeInfo().Assembly.GetType("Xamarin.Forms.VisualMarker")?.GetRuntimeProperty("Default");
        private static readonly PropertyInfo _visualProperty = typeof(VisualElement).GetRuntimeProperty("Visual");

        internal static bool IsRendererInitialized { get; set; } = IsDesignModeEnabled;

        [Obsolete]
        public static bool FixedOnMeasureBehavior { get; set; } = true;
        [Obsolete]
        public static bool FixedAndroidMotionEventHandler { get; set; } = true;

        private bool _reloadBecauseOfMissingSize;

        /// <summary>
        /// CachedImage by Daniel Luberda
        /// </summary>
        public CachedImage()
        {
            Transformations = new List<Work.ITransformation>();

            // Fix for issues with non-default visual style
            if (_visualProperty != null && _visualMarkerProperty != null)
            {
                _visualProperty.SetValue(this, _visualMarkerProperty.GetValue(null));
            }
        }




        private static void OnSourcePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!IsRendererInitialized)
            {
#if ANDROID
                if (MmImageLoading.ImageService.EnableMockImageService)
                    return;
#endif

                throw new Exception("Please call CachedImageRenderer.Init method in a platform specific project to use MmImageLoading!");
            }

            if (newValue != null)
            {
                SetInheritedBindingContext(newValue as BindableObject, bindable.BindingContext);
            }
        }






        /// <summary>
        /// The transformations property.
        /// </summary>
        public static readonly BindableProperty TransformationsProperty = BindableProperty.Create(nameof(Transformations), typeof(List<Work.ITransformation>), typeof(CachedImage), new List<Work.ITransformation>(), propertyChanged: HandleTransformationsPropertyChangedDelegate);

        /// <summary>
        /// Gets or sets the transformations.
        /// </summary>
        /// <value>The transformations.</value>
        public List<Work.ITransformation> Transformations
        {
            get => (List<Work.ITransformation>)GetValue(TransformationsProperty);
            set => SetValue(TransformationsProperty, value);
        }


        private static void HandleTransformationsPropertyChangedDelegate(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue != newValue)
            {
                if (bindable is CachedImage cachedImage && cachedImage?.Source != null)
                {
                    cachedImage.ReloadImage();
                }
            }
        }

        /// <summary>
        /// Gets or sets the cache custom key factory.
        /// </summary>
        /// <value>The cache key factory.</value>
        public ICacheKeyFactory CacheKeyFactory { get; set; }

        /// <summary>
        /// Gets or sets the custom data resolver for eg. SVG support (another nuget)
        /// </summary>
        /// <value>The custom data resolver.</value>
        public Work.IDataResolver CustomDataResolver { get; set; }

        //
        // Methods
        //
        protected override void OnBindingContextChanged()
        {
            if (Source != null)
            {
                SetInheritedBindingContext(Source, BindingContext);
            }

            base.OnBindingContextChanged();
        }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var desiredSize = base.OnMeasure(double.PositiveInfinity, double.PositiveInfinity);
            var desiredWidth = double.IsNaN(desiredSize.Request.Width) ? 0 : desiredSize.Request.Width;
            var desiredHeight = double.IsNaN(desiredSize.Request.Height) ? 0 : desiredSize.Request.Height;

            if (double.IsNaN(widthConstraint))
                widthConstraint = 0;
            if (double.IsNaN(heightConstraint))
                heightConstraint = 0;

            if (Math.Abs(desiredWidth) < double.Epsilon || Math.Abs(desiredHeight) < double.Epsilon)
                return new SizeRequest(new Size(0, 0));

            if (FixedOnMeasureBehavior)
            {
                var desiredAspect = desiredSize.Request.Width / desiredSize.Request.Height;
                var constraintAspect = widthConstraint / heightConstraint;
                var width = desiredWidth;
                var height = desiredHeight;

                if (constraintAspect > desiredAspect)
                {
                    // constraint area is proportionally wider than image
                    switch (Aspect)
                    {
                        case Aspect.AspectFit:
                        case Aspect.AspectFill:
                            height = Math.Min(desiredHeight, heightConstraint);
                            width = desiredWidth * (height / desiredHeight);
                            break;
                        case Aspect.Fill:
                            width = Math.Min(desiredWidth, widthConstraint);
                            height = desiredHeight * (width / desiredWidth);
                            break;
                    }
                }
                else if (constraintAspect < desiredAspect)
                {
                    // constraint area is proportionally taller than image
                    switch (Aspect)
                    {
                        case Aspect.AspectFit:
                        case Aspect.AspectFill:
                            width = Math.Min(desiredWidth, widthConstraint);
                            height = desiredHeight * (width / desiredWidth);
                            break;
                        case Aspect.Fill:
                            height = Math.Min(desiredHeight, heightConstraint);
                            width = desiredWidth * (height / desiredHeight);
                            break;
                    }
                }
                else
                {
                    // constraint area is same aspect as image
                    width = Math.Min(desiredWidth, widthConstraint);
                    height = desiredHeight * (width / desiredWidth);
                }

                return new SizeRequest(new Size(double.IsNaN(width) ? 0 : width, double.IsNaN(height) ? 0 : height));
            }

            if (double.IsPositiveInfinity(widthConstraint) && double.IsPositiveInfinity(heightConstraint))
            {
                return new SizeRequest(new Size(desiredWidth, desiredHeight));
            }

            if (double.IsPositiveInfinity(widthConstraint))
            {
                var factor = heightConstraint / desiredHeight;
                return new SizeRequest(new Size(desiredWidth * factor, desiredHeight * factor));
            }

            if (double.IsPositiveInfinity(heightConstraint))
            {
                var factor = widthConstraint / desiredWidth;
                return new SizeRequest(new Size(desiredWidth * factor, desiredHeight * factor));
            }

            var fitsWidthRatio = widthConstraint / desiredWidth;
            var fitsHeightRatio = heightConstraint / desiredHeight;

            if (double.IsNaN(fitsWidthRatio))
                fitsWidthRatio = 0;
            if (double.IsNaN(fitsHeightRatio))
                fitsHeightRatio = 0;

            if (Math.Abs(fitsWidthRatio) < double.Epsilon && Math.Abs(fitsHeightRatio) < double.Epsilon)
                return new SizeRequest(new Size(0, 0));

            if (Math.Abs(fitsWidthRatio) < double.Epsilon)
                return new SizeRequest(new Size(desiredWidth * fitsHeightRatio, desiredHeight * fitsHeightRatio));

            if (Math.Abs(fitsHeightRatio) < double.Epsilon)
                return new SizeRequest(new Size(desiredWidth * fitsWidthRatio, desiredHeight * fitsWidthRatio));

            var ratioFactor = Math.Min(fitsWidthRatio, fitsHeightRatio);

            return new SizeRequest(new Size(desiredWidth * ratioFactor, desiredHeight * ratioFactor));
        }


        public Action InternalReloadImage { get; set; }

        /// <summary>
        /// Reloads the image.
        /// </summary>
        public void ReloadImage() => InternalReloadImage?.Invoke();

        public Action InternalCancel { get; set; }
        /// <summary>
        /// Cancels image loading tasks
        /// </summary>
        public void Cancel() => InternalCancel?.Invoke();

        public Func<GetImageAsJpgArgs, Task<byte[]>> InternalGetImageAsJPG { get; set; }

        /// <summary>
        /// Gets the image as JPG.
        /// </summary>
        /// <returns>The image as JPG.</returns>
        public Task<byte[]> GetImageAsJpgAsync(int quality = 90, int desiredWidth = 0, int desiredHeight = 0)
        {
            if (InternalGetImageAsJPG == null)
                return null;

            return InternalGetImageAsJPG(new GetImageAsJpgArgs()
            {
                Quality = quality,
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight,
            });
        }

        public Func<GetImageAsPngArgs, Task<byte[]>> InternalGetImageAsPNG { get; set; }


        /// <summary>
        /// Gets the image as PNG
        /// </summary>
        /// <returns>The image as PNG.</returns>
        public Task<byte[]> GetImageAsPngAsync(int desiredWidth = 0, int desiredHeight = 0)
        {
            if (InternalGetImageAsPNG == null)
                return null;

            return InternalGetImageAsPNG(new GetImageAsPngArgs()
            {
                DesiredWidth = desiredWidth,
                DesiredHeight = desiredHeight,
            });
        }

        /// <summary>
        /// Invalidates cache for a specified image source.
        /// </summary>
        /// <param name="source">Image source.</param>
        /// <param name="cacheType">Cache type.</param>
        /// <param name = "removeSimilar">If set to <c>true</c> removes all image cache variants
        /// (downsampling and transformations variants)</param>
        public static async Task InvalidateCache(ImageSource source, Cache.CacheType cacheType, bool removeSimilar = false)
        {
#if (ANDROID || IOS)
            if (source is FileImageSource fileImageSource)
                await MmImageLoading.ImageService.Instance.InvalidateCacheEntryAsync(fileImageSource.File, cacheType, removeSimilar).ConfigureAwait(false);

            if (source is UriImageSource uriImageSource)
                await MmImageLoading.ImageService.Instance.InvalidateCacheEntryAsync(uriImageSource.Uri.OriginalString, cacheType, removeSimilar).ConfigureAwait(false);

            if (source is EmbeddedResourceImageSource embResourceSource)
                await MmImageLoading.ImageService.Instance.InvalidateCacheEntryAsync(embResourceSource.Uri.OriginalString, cacheType, removeSimilar).ConfigureAwait(false);
#endif
        }

        /// <summary>
        /// Invalidates cache for a specified key.
        /// </summary>
        /// <param name="key">Image key.</param>
        /// <param name="cacheType">Cache type.</param>
        /// <param name = "removeSimilar">If set to <c>true</c> removes all image cache variants
        /// (downsampling and transformations variants)</param>
        public static Task InvalidateCache(string key, CacheType cacheType, bool removeSimilar = false)
        {
#if ANDROID
            return MmImageLoading.ImageService.Instance.InvalidateCacheEntryAsync(key, cacheType, removeSimilar);
#else
            return Task.CompletedTask;
#endif
        }

        /// <summary>
        /// Occurs after image loading success.
        /// </summary>
        public event EventHandler<CachedImageEvents.SuccessEventArgs> Success;

        /// <summary>
        /// The SuccessCommandProperty.
        /// </summary>
        public static readonly BindableProperty SuccessCommandProperty = BindableProperty.Create(nameof(SuccessCommand), typeof(ICommand), typeof(CachedImage));

        /// <summary>
        /// Gets or sets the SuccessCommand.
        /// Occurs after image loading success.
        /// Command parameter: CachedImageEvents.SuccessEventArgs
        /// </summary>
        /// <value>The success command.</value>
        public ICommand SuccessCommand
        {
            get => (ICommand)GetValue(SuccessCommandProperty);
            set => SetValue(SuccessCommandProperty, value);
        }

        internal void OnSuccess(CachedImageEvents.SuccessEventArgs e)
        {
            Success?.Invoke(this, e);

            var successCommand = SuccessCommand;
            if (successCommand != null && successCommand.CanExecute(e))
                successCommand.Execute(e);
        }

        /// <summary>
        /// Occurs after image loading error.
        /// </summary>
        public event EventHandler<CachedImageEvents.ErrorEventArgs> Error;

        /// <summary>
        /// The ErrorCommandProperty.
        /// </summary>
        public static readonly BindableProperty ErrorCommandProperty = BindableProperty.Create(nameof(ErrorCommand), typeof(ICommand), typeof(CachedImage));

        /// <summary>
        /// Gets or sets the ErrorCommand.
        /// Occurs after image loading error.
        /// Command parameter: CachedImageEvents.ErrorEventArgs
        /// </summary>
        /// <value>The error command.</value>
        public ICommand ErrorCommand
        {
            get => (ICommand)GetValue(ErrorCommandProperty);
            set => SetValue(ErrorCommandProperty, value);
        }

        internal void OnError(CachedImageEvents.ErrorEventArgs e)
        {
            Error?.Invoke(this, e);

            var errorCommand = ErrorCommand;
            if (errorCommand != null && errorCommand.CanExecute(e))
                errorCommand.Execute(e);
        }

        /// <summary>
        /// Occurs after every image loading.
        /// </summary>
        public event EventHandler<CachedImageEvents.FinishEventArgs> Finish;

        /// <summary>
        /// The FinishCommandProperty.
        /// </summary>
        public static readonly BindableProperty FinishCommandProperty = BindableProperty.Create(nameof(FinishCommand), typeof(ICommand), typeof(CachedImage));

        /// <summary>
        /// Gets or sets the FinishCommand.
        /// Occurs after every image loading.
        /// Command parameter: CachedImageEvents.FinishEventArgs
        /// </summary>
        /// <value>The finish command.</value>
        public ICommand FinishCommand
        {
            get => (ICommand)GetValue(FinishCommandProperty);
            set => SetValue(FinishCommandProperty, value);
        }

        internal void OnFinish(CachedImageEvents.FinishEventArgs e)
        {
            Finish?.Invoke(this, e);

            var finishCommand = FinishCommand;
            if (finishCommand != null && finishCommand.CanExecute(e))
                finishCommand.Execute(e);
        }


        /// <summary>
        /// Occurs when an image starts downloading from web.
        /// </summary>
        public event EventHandler<CachedImageEvents.DownloadStartedEventArgs> DownloadStarted;

        /// <summary>
        /// The DownloadStartedCommandProperty.
        /// </summary>
        public static readonly BindableProperty DownloadStartedCommandProperty = BindableProperty.Create(nameof(DownloadStartedCommand), typeof(ICommand), typeof(CachedImage));

        /// <summary>
        /// Gets or sets the DownloadStartedCommand.
        ///  Occurs when an image starts downloading from web.
        /// Command parameter: DownloadStartedEventArgs
        /// </summary>
        /// <value>The download started command.</value>
        public ICommand DownloadStartedCommand
        {
            get => (ICommand)GetValue(DownloadStartedCommandProperty);
            set => SetValue(DownloadStartedCommandProperty, value);
        }

        internal void OnDownloadStarted(CachedImageEvents.DownloadStartedEventArgs e)
        {
            DownloadStarted?.Invoke(this, e);

            var downloadStartedCommand = DownloadStartedCommand;
            if (downloadStartedCommand != null && downloadStartedCommand.CanExecute(e))
                downloadStartedCommand.Execute(e);
        }

        /// <summary>
        /// This callback can be used for reading download progress
        /// </summary>
        public event EventHandler<CachedImageEvents.DownloadProgressEventArgs> DownloadProgress;

        /// <summary>
        /// The DownloadProgressCommandProperty.
        /// </summary>
        public static readonly BindableProperty DownloadProgressCommandProperty = BindableProperty.Create(nameof(DownloadProgressCommand), typeof(ICommand), typeof(CachedImage));

        /// <summary>
        /// Gets or sets the DownloadProgressCommand.
        ///  This callback can be used for reading download progress
        /// Command parameter: DownloadProgressEventArgs
        /// </summary>
        /// <value>The download started command.</value>
        public ICommand DownloadProgressCommand
        {
            get => (ICommand)GetValue(DownloadProgressCommandProperty);
            set => SetValue(DownloadProgressCommandProperty, value);
        }

        internal void OnDownloadProgress(CachedImageEvents.DownloadProgressEventArgs e)
        {
            DownloadProgress?.Invoke(this, e);

            var downloadProgressCommand = DownloadProgressCommand;
            if (downloadProgressCommand != null && downloadProgressCommand.CanExecute(e))
                downloadProgressCommand.Execute(e);
        }

        /// <summary>
        /// Called after file is succesfully written to the disk.
        /// </summary>
        public event EventHandler<CachedImageEvents.FileWriteFinishedEventArgs> FileWriteFinished;

        internal void OnFileWriteFinished(CachedImageEvents.FileWriteFinishedEventArgs e)
        {
            FileWriteFinished?.Invoke(this, e);

            var fileWriteFinishedCommand = FileWriteFinishedCommand;
            if (fileWriteFinishedCommand != null && fileWriteFinishedCommand.CanExecute(e))
                fileWriteFinishedCommand.Execute(e);
        }

        /// <summary>
        /// The cache type property.
        /// </summary>
        public static readonly BindableProperty CacheTypeProperty = BindableProperty.Create(nameof(CacheType), typeof(CacheType?), typeof(CachedImage), default(CacheType?));

        /// <summary>
        /// Set the cache storage type, (Memory, Disk, All). by default cache is set to All.
        /// </summary>
        public CacheType? CacheType
        {
            get => (CacheType?)GetValue(CacheTypeProperty);
            set => SetValue(CacheTypeProperty, value);
        }

        /// <summary>
        /// Setups the on before image loading.
        /// You can add additional logic here to configure image loader settings before loading
        /// </summary>
        /// <param name="imageLoader">Image loader.</param>
        protected internal virtual void SetupOnBeforeImageLoading(Work.TaskParameter imageLoader)
        {
        }

        /// <summary>
        /// Setups the on before image loading.
        /// You can add additional logic here to configure image loader settings before loading
        /// </summary>
        /// <param name="imageLoader">Image loader.</param>
        /// <param name="source">Source.</param>
        /// <param name="loadingPlaceholderSource">Loading placeholder source.</param>
        /// <param name="errorPlaceholderSource">Error placeholder source.</param>
        protected internal virtual void SetupOnBeforeImageLoading(out Work.TaskParameter imageLoader, IImageSourceBinding source, IImageSourceBinding loadingPlaceholderSource, IImageSourceBinding errorPlaceholderSource)
        {
#if (ANDROID || IOS)
            if (source.ImageSource == Work.ImageSource.Url)
            {
                imageLoader = MmImageLoading.ImageService.Instance.LoadUrl(source.Path, CacheDuration);
            }
            else if (source.ImageSource == Work.ImageSource.CompiledResource)
            {
                imageLoader = MmImageLoading.ImageService.Instance.LoadCompiledResource(source.Path);
            }
            else if (source.ImageSource == Work.ImageSource.ApplicationBundle)
            {
                imageLoader = MmImageLoading.ImageService.Instance.LoadFileFromApplicationBundle(source.Path);
            }
            else if (source.ImageSource == Work.ImageSource.Filepath)
            {
                imageLoader = MmImageLoading.ImageService.Instance.LoadFile(source.Path);
            }
            else if (source.ImageSource == Work.ImageSource.Stream)
            {
                imageLoader = MmImageLoading.ImageService.Instance.LoadStream(source.Stream);
            }
            else if (source.ImageSource == Work.ImageSource.EmbeddedResource)
            {
                imageLoader = MmImageLoading.ImageService.Instance.LoadEmbeddedResource(source.Path);
            }
            else

            {
                imageLoader = null;
                return;
            }
#else
            imageLoader = null;
            return;
#endif


            var widthRequest = (int)(double.IsPositiveInfinity(WidthRequest) ? 0 : Math.Max(0, WidthRequest));
            var heightRequest = (int)(double.IsPositiveInfinity(HeightRequest) ? 0 : Math.Max(0, HeightRequest));
            var width = (int)(double.IsPositiveInfinity(Width) ? 0 : Math.Max(0, Width));
            var height = (int)(double.IsPositiveInfinity(Height) ? 0 : Math.Max(0, Height));

            // CustomKeyFactory
            if (CacheKeyFactory != null)
            {
                var bindingContext = BindingContext;
                imageLoader?.CacheKey(CacheKeyFactory.GetKey(Source, bindingContext));
            }

            // LoadingPlaceholder
            if (LoadingPlaceholder != null)
            {

                if (loadingPlaceholderSource != null)
                    imageLoader?.LoadingPlaceholder(loadingPlaceholderSource.Path, loadingPlaceholderSource.ImageSource);
            }

            // ErrorPlaceholder
            if (ErrorPlaceholder != null)
            {
                if (errorPlaceholderSource != null)
                    imageLoader?.ErrorPlaceholder(errorPlaceholderSource.Path, errorPlaceholderSource.ImageSource);
            }

            // Enable vector image source
            var vect1 = Source as IVectorImageSource;
            var vect2 = LoadingPlaceholder as IVectorImageSource;
            var vect3 = ErrorPlaceholder as IVectorImageSource;

            if (vect1 != null || vect2 != null || vect3 != null)
            {
                if (widthRequest == 0 && heightRequest == 0 && width == 0 && height == 0)
                {
                    _reloadBecauseOfMissingSize = true;
                    imageLoader = null;
                    return;
                }

                var isWidthHeightRequestSet = widthRequest > 0 || heightRequest > 0;

                if (vect1 != null)
                {
                    var newVect = vect1.Clone();

                    if (newVect.VectorWidth == 0 && newVect.VectorHeight == 0)
                    {
                        newVect.VectorWidth = isWidthHeightRequestSet ? widthRequest : width;
                        newVect.VectorHeight = isWidthHeightRequestSet ? heightRequest : height;
                        newVect.UseDipUnits = true;
                    }

                    imageLoader?.WithCustomDataResolver(newVect.GetVectorDataResolver());
                }
                if (vect2 != null)
                {
                    var newVect = vect2.Clone();

                    if (newVect.VectorWidth == 0 && newVect.VectorHeight == 0)
                    {
                        newVect.VectorWidth = isWidthHeightRequestSet ? widthRequest : width;
                        newVect.VectorHeight = isWidthHeightRequestSet ? heightRequest : height;
                        newVect.UseDipUnits = true;
                    }

                    imageLoader?.WithCustomLoadingPlaceholderDataResolver(newVect.GetVectorDataResolver());
                }
                if (vect3 != null)
                {
                    var newVect = vect3.Clone();

                    if (newVect.VectorWidth == 0 && newVect.VectorHeight == 0)
                    {
                        newVect.VectorWidth = isWidthHeightRequestSet ? widthRequest : width;
                        newVect.VectorHeight = isWidthHeightRequestSet ? heightRequest : height;
                        newVect.UseDipUnits = true;
                    }

                    imageLoader?.WithCustomErrorPlaceholderDataResolver(newVect.GetVectorDataResolver());
                }
            }
            if (CustomDataResolver != null)
            {
                imageLoader?.WithCustomDataResolver(CustomDataResolver);
                imageLoader?.WithCustomLoadingPlaceholderDataResolver(CustomDataResolver);
                imageLoader?.WithCustomErrorPlaceholderDataResolver(CustomDataResolver);
            }

            // Downsample
            if (DownsampleToViewSize && (widthRequest > 0 || heightRequest > 0))
            {
                imageLoader?.DownSampleInDip(widthRequest, heightRequest);
            }
            else if (DownsampleToViewSize && (width > 0 || height > 0))
            {
                imageLoader?.DownSampleInDip(width, height);
            }
            else if ((int)DownsampleHeight != 0 || (int)DownsampleWidth != 0)
            {
                if (DownsampleUseDipUnits)
                    imageLoader?.DownSampleInDip((int)DownsampleWidth, (int)DownsampleHeight);
                else
                    imageLoader?.DownSample((int)DownsampleWidth, (int)DownsampleHeight);
            }
            else if (DownsampleToViewSize)
            {
                _reloadBecauseOfMissingSize = true;
                imageLoader = null;

                return;
            }

            // RetryCount
            if (RetryCount > 0)
            {
                imageLoader?.Retry(RetryCount, RetryDelay);
            }

            if (BitmapOptimizations.HasValue)
                imageLoader?.BitmapOptimizations(BitmapOptimizations.Value);

            // FadeAnimation
            if (FadeAnimationEnabled.HasValue)
                imageLoader?.FadeAnimation(FadeAnimationEnabled.Value, duration: FadeAnimationDuration);

            // FadeAnimationForCachedImages
            if (FadeAnimationEnabled.HasValue && FadeAnimationForCachedImages.HasValue)
                imageLoader?.FadeAnimation(FadeAnimationEnabled.Value, FadeAnimationForCachedImages.Value, FadeAnimationDuration);

            // TransformPlaceholders
            if (TransformPlaceholders.HasValue)
                imageLoader?.TransformPlaceholders(TransformPlaceholders.Value);

            // Transformations
            if (Transformations != null && Transformations.Count > 0)
            {
                imageLoader?.Transform(Transformations);
            }

            if (InvalidateLayoutAfterLoaded.HasValue)
                imageLoader?.InvalidateLayout(InvalidateLayoutAfterLoaded.Value);

            imageLoader?.WithPriority(LoadingPriority);
            if (CacheType.HasValue)
            {
                imageLoader?.WithCache(CacheType.Value);
            }

            if (LoadingDelay.HasValue)
            {
                imageLoader?.Delay(LoadingDelay.Value);
            }

            imageLoader?.DownloadStarted((downloadInformation) => OnDownloadStarted(new CachedImageEvents.DownloadStartedEventArgs(downloadInformation)));
            imageLoader?.DownloadProgress((progress) => OnDownloadProgress(new CachedImageEvents.DownloadProgressEventArgs(progress)));
            imageLoader?.FileWriteFinished((fileWriteInfo) => OnFileWriteFinished(new CachedImageEvents.FileWriteFinishedEventArgs(fileWriteInfo)));
            imageLoader?.Error((exception) => OnError(new CachedImageEvents.ErrorEventArgs(exception)));
            imageLoader?.Finish((work) => OnFinish(new CachedImageEvents.FinishEventArgs(work)));
            imageLoader?.Success((imageInformation, loadingResult) => OnSuccess(new CachedImageEvents.SuccessEventArgs(imageInformation, loadingResult)));

            SetupOnBeforeImageLoading(imageLoader);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (_reloadBecauseOfMissingSize)
            {
                _reloadBecauseOfMissingSize = false;

                if (width <= 0 && height <= 0)
                {
#if (ANDROID || IOS)
                    MmImageLoading.ImageService.Instance.Config.Logger?.Error("Couldn't read view size for auto sizing");
#endif
                }

                ReloadImage();
            }
        }
    }
}

