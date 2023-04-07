using System;
namespace MmImageLoading.Exceptions
{
    
    public class DownloadException : Exception
    { 
        public DownloadException(string message) : base(message)
        {
        }
    }
}
