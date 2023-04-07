using System.Threading;
using System.Threading.Tasks;
using MmImageLoading.Cache;
using MmImageLoading.Config;
using MmImageLoading.Work;

namespace MmImageLoading.DataResolvers
{
    public class UrlDataResolver : IDataResolver
    {
        public UrlDataResolver(Configuration configuration)
        {
            Configuration = configuration;
        }

        protected IDownloadCache DownloadCache => Configuration.DownloadCache;
        protected Configuration Configuration { get; private set; }

        public async virtual Task<DataResolverResult> Resolve(string identifier, TaskParameter parameters, CancellationToken token)
        {

            var downloadedData = await DownloadCache.DownloadAndCacheIfNeededAsync(identifier, parameters, Configuration, token).ConfigureAwait(false);

            if (token.IsCancellationRequested)
            {
                downloadedData?.ImageStream.TryDispose();
                token.ThrowIfCancellationRequested();
            }

            var imageInformation = new ImageInformation();
            imageInformation.SetPath(identifier);
            imageInformation.SetFilePath(downloadedData?.FilePath);

            return new DataResolverResult(
                downloadedData?.ImageStream, downloadedData.RetrievedFromDiskCache ? LoadingResult.DiskCache : LoadingResult.Internet, imageInformation);
        }
    }
}
