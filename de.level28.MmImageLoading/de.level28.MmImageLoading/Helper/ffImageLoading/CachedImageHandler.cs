using Microsoft.Maui.Handlers;


namespace MmImageLoading.MAUI
{
#if (ANDROID || IOS)
    public partial class CachedImageHandler
    {

        public static PropertyMapper<ICachedImage, CachedImageHandler> CustomEntryMapper = new PropertyMapper<ICachedImage, CachedImageHandler>(ViewHandler.ViewMapper)
        {
            [nameof(ICachedImage.Source)] = MapSource,
            [nameof(ICachedImage.Aspect)] = MapAspect
        };


        public CachedImageHandler() : base(CustomEntryMapper)
        {

        }

        public CachedImageHandler(PropertyMapper? mapper = null) : base(mapper ?? CustomEntryMapper)
        {

        }
    }
#endif
}

