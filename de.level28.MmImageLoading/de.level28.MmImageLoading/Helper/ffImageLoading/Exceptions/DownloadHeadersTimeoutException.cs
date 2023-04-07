using System;

namespace MmImageLoading.Exceptions
{
    
    public class DownloadHeadersTimeoutException : Exception
    {
        public DownloadHeadersTimeoutException() : base("Headers timeout")
        {
        }
    }
}
