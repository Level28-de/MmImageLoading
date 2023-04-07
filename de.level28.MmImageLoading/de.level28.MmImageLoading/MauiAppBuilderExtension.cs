using MmImageLoading.MAUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace de.level28.MmImageLoading
{
    public static class MauiAppBuilderExtension
    {

#if (ANDROID || IOS)
        public static MauiAppBuilder UseMmImageLoading(this MauiAppBuilder builder)
        {
            return builder.ConfigureMauiHandlers(collection =>
            {
                collection.AddHandler(typeof(CachedImage), typeof(CachedImageHandler));
            });
        }
#endif
    }
}
