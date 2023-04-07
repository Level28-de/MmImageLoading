using System;
using MmImageLoading.Work;

namespace MmImageLoading.Args
{
    
    public class FinishEventArgs : EventArgs
    {
        public FinishEventArgs(IScheduledWork scheduledWork)
        {
            ScheduledWork = scheduledWork;
        }

        public IScheduledWork ScheduledWork { get; private set; }
    }
}
