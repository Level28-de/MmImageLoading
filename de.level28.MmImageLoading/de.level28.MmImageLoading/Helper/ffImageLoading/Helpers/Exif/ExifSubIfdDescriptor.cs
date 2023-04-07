using System;
using System.Collections.Generic;

namespace MmImageLoading.Helpers.Exif
{
    internal class ExifSubIfdDescriptor : ExifDescriptorBase<ExifSubIfdDirectory>
    {
        public ExifSubIfdDescriptor(ExifSubIfdDirectory directory) : base(directory)
        {
        }
    }
}
