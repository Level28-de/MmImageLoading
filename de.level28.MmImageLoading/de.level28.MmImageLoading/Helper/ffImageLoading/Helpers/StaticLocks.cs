using System;
using System.Threading;

namespace MmImageLoading.Helpers
{
	public static class StaticLocks
	{
		public static SemaphoreSlim DecodingLock { get; set; }
	}
}
