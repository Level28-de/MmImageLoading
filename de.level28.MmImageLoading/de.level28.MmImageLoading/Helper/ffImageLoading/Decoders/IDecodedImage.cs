using System;

namespace MmImageLoading
{
    public interface IDecodedImage<TNativeImageContainer>
    {
        bool IsAnimated { get; }

        TNativeImageContainer Image { get; set; }

        IAnimatedImage<TNativeImageContainer>[] AnimatedImages { get; }
    }
}
