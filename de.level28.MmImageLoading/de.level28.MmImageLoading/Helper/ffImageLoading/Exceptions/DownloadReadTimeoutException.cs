using System;
namespace MmImageLoading.Exceptions
{
    
    public class DownloadReadTimeoutException : Exception
    {
        public DownloadReadTimeoutException() : base("Read timeout")
        {
        }
    }
}
