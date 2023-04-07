using System;
using System.Collections.Generic;
using System.Text;
using SkiaSharp.Views.iOS;
using UIKit;

namespace WebP.Touch
{
    internal class WebPCodec
    {
        public WebPCodec()
        {
        }

        internal UIImage Decode(Stream stream)
        {
            return SkiaSharp.SKBitmap.Decode(stream).ToUIImage();
        }
    }
}
