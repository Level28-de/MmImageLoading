using System;
using MmImageLoading.Config;
using MmImageLoading.Work;

namespace MmImageLoading.DataResolvers
{
    public class WrappedDataResolverFactory : IDataResolverFactory
    {
        readonly IDataResolverFactory _factory;

        public WrappedDataResolverFactory(IDataResolverFactory factory)
        {
            _factory = factory;
        }

        public IDataResolver GetResolver(string identifier, MmImageLoading.Work.ImageSource source, TaskParameter parameters, Configuration configuration)
        {
            return new WrappedDataResolver(_factory.GetResolver(identifier, source, parameters, configuration));
        }
    }
}
