using System;
using MmImageLoading.Config;

namespace MmImageLoading.Work
{
    public interface IDataResolverFactory
    {
        IDataResolver GetResolver(string identifier, ImageSource source, TaskParameter parameters, Configuration configuration);
    }
}
