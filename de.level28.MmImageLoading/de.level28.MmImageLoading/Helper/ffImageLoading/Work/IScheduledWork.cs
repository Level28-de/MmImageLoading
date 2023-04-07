using System;

namespace MmImageLoading.Work
{
    public interface IScheduledWork
    {
        void Cancel();

        bool IsCancelled { get; }

        bool IsCompleted { get; }
    }
}

