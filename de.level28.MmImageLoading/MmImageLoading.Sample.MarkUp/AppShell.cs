

namespace MmImageLoading.Sample.MarkUp;

class AppShell : Shell
{
    static readonly IReadOnlyDictionary<Type, string> pageRouteMappingDictionary = new Dictionary<Type, string>(new[]
    {
        CreateRoutePageMapping<MainPage, MainViewModel>(),
        CreateRoutePageMapping<LocalPage, LocalViewModel>(),
        CreateRoutePageMapping<OnlinePage, OnlineViewModel>(),
    });

    public AppShell(MainPage newsPage)
    {
        Items.Add(newsPage);
    }

    public static string GetRoute<TPage, TViewModel>() where TPage : BaseContentPage<TViewModel>
                                                        where TViewModel : BaseViewModel
    {
        if (!pageRouteMappingDictionary.TryGetValue(typeof(TPage), out var route))
        {
            throw new KeyNotFoundException($"No map for ${typeof(TPage)} was found on navigation mappings. Please register your ViewModel in {nameof(AppShell)}.{nameof(pageRouteMappingDictionary)}");
        }

        return route;
    }

    static KeyValuePair<Type, string> CreateRoutePageMapping<TPage, TViewModel>() where TPage : BaseContentPage<TViewModel>
                                                                                    where TViewModel : BaseViewModel
    {
        var route = CreateRoute();
        Routing.RegisterRoute(route, typeof(TPage));

        return new KeyValuePair<Type, string>(typeof(TPage), route);

        static string CreateRoute()
        {
            if (typeof(TPage) == typeof(MainPage))
            {
                return $"//{nameof(MainPage)}";
            }
            if (typeof(TPage) == typeof(LocalPage))
            {
                return $"//{nameof(MainPage)}/{nameof(LocalPage)}";
            }
            if (typeof(TPage) == typeof(OnlinePage))
            {
                return $"//{nameof(MainPage)}/{nameof(OnlinePage)}";
            }
            throw new NotSupportedException($"{typeof(TPage)} Not Implemented in {nameof(pageRouteMappingDictionary)}");
        }
    }
}