using System;
using MmImageLoading.Work;

namespace MmImageLoading.MAUI
{
    public static class CachedImageEvents
    {
        public class ErrorEventArgs : MmImageLoading.Args.ErrorEventArgs
        {
            public ErrorEventArgs(Exception exception) : base(exception) { }
        }

        public class SuccessEventArgs : MmImageLoading.Args.SuccessEventArgs
        {
            public SuccessEventArgs(ImageInformation imageInformation, LoadingResult loadingResult) : base(imageInformation, loadingResult) { }
        }

        public class FinishEventArgs : MmImageLoading.Args.FinishEventArgs
        {
            public FinishEventArgs(IScheduledWork scheduledWork) : base(scheduledWork) { }
        }

        public class DownloadStartedEventArgs : MmImageLoading.Args.DownloadStartedEventArgs
        {
            public DownloadStartedEventArgs(DownloadInformation downloadInformation) : base(downloadInformation) { }
        }

        public class DownloadProgressEventArgs : MmImageLoading.Args.DownloadProgressEventArgs
        {
            public DownloadProgressEventArgs(DownloadProgress downloadProgress) : base(downloadProgress) { }
        }

        public class FileWriteFinishedEventArgs : MmImageLoading.Args.FileWriteFinishedEventArgs
        {
            public FileWriteFinishedEventArgs(FileWriteInfo fileWriteInfo) : base(fileWriteInfo) { }
        }
    }
}

