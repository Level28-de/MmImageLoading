using System;
using System.IO;
using System.Threading.Tasks;
using MmImageLoading.Work;

namespace MmImageLoading.Decoders
{
    public interface IDecoder<TDecoderContainer>
    {
        Task<IDecodedImage<TDecoderContainer>> DecodeAsync(Stream stream, string path, MmImageLoading.Work.ImageSource source, ImageInformation imageInformation, TaskParameter parameters);
    }
}
