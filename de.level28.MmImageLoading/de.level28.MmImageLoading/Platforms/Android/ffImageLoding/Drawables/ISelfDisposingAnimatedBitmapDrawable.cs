using System;
using Android.Graphics;

namespace MmImageLoading.Drawables
{
	public interface ISelfDisposingAnimatedBitmapDrawable : ISelfDisposingBitmapDrawable
	{
		IAnimatedImage<Bitmap>[] AnimatedImages { get; }
	}
}
