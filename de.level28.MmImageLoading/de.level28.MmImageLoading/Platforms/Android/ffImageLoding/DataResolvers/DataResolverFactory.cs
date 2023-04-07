using System;
using MmImageLoading.Config;
using MmImageLoading.Work;
using ImageSource = MmImageLoading.Work.ImageSource;

namespace MmImageLoading.DataResolvers
{
    public class DataResolverFactory : IDataResolverFactory
    {
        public IDataResolver GetResolver(string identifier, ImageSource source, TaskParameter parameters, Configuration configuration)
        {
            switch (source)
            {
                case ImageSource.ApplicationBundle:
                    return new BundleDataResolver();
                case ImageSource.CompiledResource:
                    return new ResourceDataResolver();
                case ImageSource.Filepath:
                    return new FileDataResolver();
                case ImageSource.Url:
                    if (!string.IsNullOrWhiteSpace(identifier) && identifier.IsDataUrl())
                        return new DataUrlResolver();
                    return new UrlDataResolver(configuration);
                case ImageSource.Stream:
                    return new StreamDataResolver();
                case ImageSource.EmbeddedResource:
                    return new EmbeddedResourceResolver();
                default:
                    throw new NotSupportedException("Unknown type of ImageSource");
            }
        }
    }
}
