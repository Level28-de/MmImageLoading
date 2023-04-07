using System.Threading.Tasks;
using System.Threading;
using MmImageLoading.Work;
using MmImageLoading.Config;

namespace MmImageLoading.Cache
{
    
	public interface IDownloadCache
	{
        Task<CacheStream> DownloadAndCacheIfNeededAsync (string url, TaskParameter parameters, Configuration configuration, CancellationToken token);
	}
}

