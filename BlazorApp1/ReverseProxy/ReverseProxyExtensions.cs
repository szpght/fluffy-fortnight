namespace BlazorApp1.ReverseProxy;

static class ReverseProxyExtensions {
    public static void AddProxy(this IServiceCollection services) {
        var itemsProvider = new ItemsProvider();
        var (routes, clusters) = itemsProvider.CreateConfigs(itemsProvider.Config.Components);

        services.AddSingleton(itemsProvider);
        services.AddReverseProxy()
            .LoadFromMemory(routes, clusters);
    }
}