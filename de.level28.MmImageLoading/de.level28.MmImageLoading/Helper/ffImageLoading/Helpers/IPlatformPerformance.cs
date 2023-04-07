using System;
namespace MmImageLoading
{
    [Preserve(AllMembers = true)]
    public interface IPlatformPerformance
    {
        int GetCurrentManagedThreadId();

        int GetCurrentSystemThreadId();

        string GetMemoryInfo();
    }
}

