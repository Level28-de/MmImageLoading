using MmImageLoading.Work;
using System.Linq;

namespace MmImageLoading.Concurrency
{
    public class PendingTasksQueue : SimplePriorityQueue<IImageLoaderTask, int>
    {
        public IImageLoaderTask FirstOrDefaultByRawKey(string rawKey)
        {
            lock (_queue)
            {
                return _queue.FirstOrDefault(v => v.Data?.KeyRaw == rawKey)?.Data;
            }
        }
    }
}
