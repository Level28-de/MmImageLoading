using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MmImageLoading.Helpers.Exif;

namespace MmImageLoading.Helpers
{
    internal static class ExifHelper
    {
        //const int MOTOROLA_TIFF_MAGIC_NUMBER = 0x4D4D;
        //const int INTEL_TIFF_MAGIC_NUMBER = 0x4949;

        static readonly ExifReader _exifReader = new ExifReader();

        public static IList<MmImageLoading.Helpers.Exif.Directory> Read(Stream stream)
        {
            if (stream.Position != 0)
                stream.Position = 0;


            var segmentTypes = _exifReader.SegmentTypes;
            var segments = JpegSegmentReader.ReadSegments(new SequentialStreamReader(stream), segmentTypes);

            var directories = new List<MmImageLoading.Helpers.Exif.Directory>();

            var readerSegmentTypes = _exifReader.SegmentTypes;
            var readerSegments = segments.Where(s => readerSegmentTypes.Contains(s.Type));
            directories.AddRange(_exifReader.ReadJpegSegments(readerSegments));

            return directories;
        }
    }
}
