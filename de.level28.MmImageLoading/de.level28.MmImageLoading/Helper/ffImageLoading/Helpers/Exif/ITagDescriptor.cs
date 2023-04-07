using System;

namespace MmImageLoading.Helpers.Exif
{
    internal interface ITagDescriptor
    {
        string GetDescription(int tagType);
    }
}
