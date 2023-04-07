using System;

namespace MmImageLoading.Args
{
    
    public class ErrorEventArgs : EventArgs
    {
        public ErrorEventArgs(Exception exception)
        {
        	Exception = exception;
        }

        public Exception Exception { get; private set; }
    }
}
