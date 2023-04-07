using System;
using System.IO;
using System.Threading.Tasks;
using MmImageLoading.Mock;
using MmImageLoading.Work;

namespace MmImageLoading.Decoders
{
    public class MockDecoder : IDecoder<MockBitmap>
    {
        public Task<IDecodedImage<MockBitmap>> DecodeAsync(Stream stream, string path, Work.ImageSource source, ImageInformation imageInformation, TaskParameter parameters)
        {
            var result = new DecodedImage<MockBitmap>()
            {
                Image = new MockBitmap(),
            };

            return Task.FromResult<IDecodedImage<MockBitmap>>(result);
        }
    }
}
