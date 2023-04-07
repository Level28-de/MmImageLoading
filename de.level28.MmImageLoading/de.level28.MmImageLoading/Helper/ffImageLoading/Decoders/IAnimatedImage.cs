using System;

namespace MmImageLoading
{
    public interface IAnimatedImage<TNativeImageContainer>
    {
        int Delay { get; set; }

        TNativeImageContainer Image { get; set; }
    }
}
