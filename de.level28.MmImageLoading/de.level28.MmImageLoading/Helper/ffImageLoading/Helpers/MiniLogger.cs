using System;
using System.Diagnostics;

namespace MmImageLoading.Helpers
{
    public class MiniLogger: IMiniLogger
    {
        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Error(string errorMessage)
        {
            Console.WriteLine(errorMessage);
        }

        public void Error(string errorMessage, Exception ex)
        {
            Error(errorMessage + Environment.NewLine + ex.ToString());
        }
    }
}

