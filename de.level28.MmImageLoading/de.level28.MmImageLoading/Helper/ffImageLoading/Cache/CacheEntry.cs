using System;

namespace MmImageLoading.Cache
{
    public readonly struct CacheEntry
    {
        public readonly DateTime Origin;
        public readonly TimeSpan TimeToLive;
        public readonly string FileName;

        public CacheEntry(DateTime o, TimeSpan ttl, string fileName)
        {
            Origin = o;
            TimeToLive = ttl;
            FileName = fileName;
        }
    }
}
