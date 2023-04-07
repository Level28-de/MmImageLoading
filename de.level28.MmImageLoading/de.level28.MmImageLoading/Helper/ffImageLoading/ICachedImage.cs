using MmImageLoading.MAUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MmImageLoading.MAUI
{
    public interface ICachedImage : IView
    {
        /// <summary>
        /// The aspect property.
        /// </summary>
        Aspect Aspect { get; set; } // Aspect.AspectFit

        /// <summary>
        /// The is loading property key.
        /// </summary>
        bool IsLoadingKey { get; set; }

        /// <summary>
        /// The is loading property.
        /// </summary>
        bool IsLoading { get; set; }

        /// <summary>
        /// The is opaque property.
        /// </summary>
        bool IsOpaque { get; set; }

        /// <summary>
        /// The source property.
        /// </summary>
        ImageSource Source { get; set; } // ImageSource

        /// <summary>
        /// The retry count property.
        /// </summary>
        int RetryCount { get; set; } // 3

        /// <summary>
        /// The retry delay property.
        /// </summary>
        int RetryDelay { get; set; } // 250

        /// <summary>
        /// The loading delay property.
        /// </summary>
        int? LoadingDelay { get; set; }

        /// <summary>
        /// The downsample width property.
        /// </summary>
        double DownsampleWidth { get; set; } // 0d

        /// <summary>
        /// The downsample height property.
        /// </summary>
        double DownsampleHeight { get; set; } // 0d

        /// <summary>
        /// The downsample to view size property.
        /// </summary>
        bool DownsampleToViewSize { get; set; } // false

        /// <summary>
        /// The downsample use dip units property.
        /// </summary>
        bool DownsampleUseDipUnits { get; set; }

        /// <summary>
        /// The cache duration property.
        /// </summary>
        TimeSpan? CacheDuration { get; set; }

        /// <summary>
        /// The loading priority property.
        /// </summary>
        Work.LoadingPriority LoadingPriority { get; set; } // Work.LoadingPriority.Normal

        /// <summary>
        /// The bitmap optimizations property.
        /// </summary>
        bool? BitmapOptimizations { get; set; }

        /// <summary>
        /// The fade animation enabled property.
        /// </summary>
        bool? FadeAnimationEnabled { get; set; }

        /// <summary>
        /// The fade animation for cached images enabled property.
        /// </summary>
        bool? FadeAnimationForCachedImages { get; set; }

        /// <summary>
        /// The fade animation duration property.
        /// </summary>
        int? FadeAnimationDuration { get; set; }

        /// <summary>
        /// The loading placeholder property.
        /// </summary>
        ImageSource LoadingPlaceholder { get; set; } // check: coerceValue: CoerceImageSource);

        /// <summary>
        /// The error placeholder property.
        /// </summary>
        ImageSource ErrorPlaceholder { get; set; } // coerceValue: CoerceImageSource);

        /// <summary>
        /// The TransformPlaceholders property.
        /// </summary>
        bool? TransformPlaceholders { get; set; }

        /// <summary>
        /// The invalidate layout after loaded property.
        /// </summary>
        bool? InvalidateLayoutAfterLoaded { get; set; }

        /// <summary>
        /// The FileWriteFinishedCommandProperty.
        /// </summary>
        ICommand FileWriteFinishedCommand { get; set; }



        Action InternalReloadImage { get; set; }
        Action InternalCancel { get; set; }
        Func<GetImageAsJpgArgs, Task<byte[]>> InternalGetImageAsJPG { get; set; }
        Func<GetImageAsPngArgs, Task<byte[]>> InternalGetImageAsPNG { get; set; }
    }
}
