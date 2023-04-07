using System;
using System.Diagnostics;

namespace MmImageLoading.Helpers
{
    
    public interface IMiniLogger
    {
        void Debug(string message);

		void Error(string errorMessage);

        void Error(string errorMessage, Exception ex);
    }
}

