﻿using System;
using System.Threading.Tasks;
using MmImageLoading.Work;
using System.IO;
using MmImageLoading.Targets;
using MmImageLoading.Drawables;
using Android.Widget;
using ImageSource = MmImageLoading.Work.ImageSource;

namespace MmImageLoading
{
    /// <summary>
    /// TaskParameterPlatformExtensions
    /// </summary>
    public static class TaskParameterPlatformExtensions
    {
        /// <summary>
        /// Loads the image into PNG Stream
        /// </summary>
        /// <returns>The PNG Stream async.</returns>
        /// <param name="parameters">Parameters.</param>
        public static async Task<Stream> AsPNGStreamAsync(this TaskParameter parameters)
        {
            using (var result = await AsBitmapDrawableAsync(parameters).ConfigureAwait(false))
            {
                var stream = await result.AsPngStreamAsync().ConfigureAwait(false);

                return stream;
            }
        }

        /// <summary>
        /// Loads the image into JPG Stream
        /// </summary>
        /// <returns>The JPG Stream async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="quality">Quality.</param>
        public static async Task<Stream> AsJPGStreamAsync(this TaskParameter parameters, int quality = 80)
        {
            using (var result = await AsBitmapDrawableAsync(parameters).ConfigureAwait(false))
            {
                var stream = await result.AsJpegStreamAsync(quality).ConfigureAwait(false);
                result.SetIsDisplayed(false);

                return stream;
            }
        }

        /// <summary>
        /// Loads and gets BitmapDrawable using defined parameters.
        /// IMPORTANT: you should call SetNoLongerDisplayed method if drawable is no longer displayed
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>The bitmap drawable async.</returns>
        /// <param name="parameters">Parameters.</param>
        public static Task<SelfDisposingBitmapDrawable> AsBitmapDrawableAsync(this TaskParameter parameters)
        {
            var target = new BitmapTarget();
            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            var tcs = new TaskCompletionSource<SelfDisposingBitmapDrawable>();

            parameters
                .Error(ex =>
                {
                    tcs.TrySetException(ex);
                    userErrorCallback?.Invoke(ex);
                })
                .Finish(scheduledWork =>
                {
                    finishCallback?.Invoke(scheduledWork);
                    tcs.TrySetResult(target.BitmapDrawable);
                });

            if (parameters.Source != ImageSource.Stream && string.IsNullOrWhiteSpace(parameters.Path))
            {
                target.SetAsEmpty(null);
                parameters.TryDispose();
                return null;
            }

            var task = ImageService.CreateTask(parameters, target);
            ImageService.Instance.LoadImage(task);

            return tcs.Task;
        }

        /// <summary>
        /// Loads the image into given ImageView using defined parameters.
        /// </summary>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static IScheduledWork Into(this TaskParameter parameters, ImageView imageView)
        {
            var target = new ImageViewTarget(imageView);
            return parameters.Into(target);
        }

        /// <summary>
        /// Loads the image into given ImageView using defined parameters.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>An awaitable Task.</returns>
        /// <param name="parameters">Parameters for loading the image.</param>
        /// <param name="imageView">Image view that should receive the image.</param>
        public static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, ImageView imageView)
        {
            return parameters.IntoAsync(param => param.Into(imageView));
        }

        /// <summary>
        /// Loads the image into given target using defined parameters.
        /// </summary>
        /// <returns>The into.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="target">Target.</param>
        /// <typeparam name="TImageView">The 1st type parameter.</typeparam>
        public static IScheduledWork Into<TImageView>(this TaskParameter parameters, ITarget<SelfDisposingBitmapDrawable, TImageView> target) where TImageView : class
        {
            if (parameters.Source != ImageSource.Stream && string.IsNullOrWhiteSpace(parameters.Path))
            {
                target.SetAsEmpty(null);
                parameters.TryDispose();
                return null;
            }

            var task = ImageService.CreateTask(parameters, target);
            ImageService.Instance.LoadImage(task);
            return task;
        }

        /// <summary>
        /// Loads the image into given target using defined parameters.
        /// IMPORTANT: It throws image loading exceptions - you should handle them
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="parameters">Parameters.</param>
        /// <param name="target">Target.</param>
        /// <typeparam name="TImageView">The 1st type parameter.</typeparam>
        public static Task<IScheduledWork> IntoAsync<TImageView>(this TaskParameter parameters, ITarget<SelfDisposingBitmapDrawable, TImageView> target) where TImageView : class
        {
            return parameters.IntoAsync(param => param.Into(target));
        }

        private static Task<IScheduledWork> IntoAsync(this TaskParameter parameters, Action<TaskParameter> into)
        {
            var userErrorCallback = parameters.OnError;
            var finishCallback = parameters.OnFinish;
            var tcs = new TaskCompletionSource<IScheduledWork>();

            parameters
                .Error(ex =>
                {
                    tcs.TrySetException(ex);
                    userErrorCallback?.Invoke(ex);
                })
                .Finish(scheduledWork =>
                {
                    finishCallback?.Invoke(scheduledWork);
                    tcs.TrySetResult(scheduledWork);
                });

            into(parameters);

            return tcs.Task;
        }
    }
}
