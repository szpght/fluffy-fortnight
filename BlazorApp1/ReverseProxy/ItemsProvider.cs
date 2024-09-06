using System.Collections.Immutable;
using System.Text.Json;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

namespace BlazorApp1.ReverseProxy;

public class ItemsProvider {
    public ConfigFile Config { get; private set; }

    public ItemsProvider() {
        LoadConfig();
    }

    public void Reload(InMemoryConfigProvider configProvider) {
        LoadConfig();
        var (routes, clusters) = CreateConfigs(Config.Components);
        configProvider.Update(routes, clusters);
    }

    private void LoadConfig() {
        var configFile = File.ReadAllText("config.json");
        
        var config = JsonSerializer.Deserialize(configFile, SourceGenerationContext.Default.ConfigFile)
                     ?? throw new Exception("Deserialized config is null - incorrect config.json");
        
        Config = config with {
            Components = config.Components
                .OrderBy(x => x.Name)
                .ToImmutableList()
        };
    }

    public void Toggle(Item item, InMemoryConfigProvider configProvider) {
        var items = Config.Components.Replace(item, item with {IsLocal = !item.IsLocal});
        var (routes, clusters) = CreateConfigs(items);
        configProvider.Update(routes, clusters);
        Config = Config with {Components = items};
        var jsonFile =
            JsonSerializer.SerializeToUtf8Bytes(Config, SourceGenerationContext.Default.ConfigFile);
        File.WriteAllBytes("config.json.new", jsonFile);
        File.Replace("config.json.new", "config.json", "config.json.bak");
    }

    public (IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters) CreateConfigs(
        ImmutableList<Item> items) {
        var routes = new List<RouteConfig>(items.Count);
        var clusters = new List<ClusterConfig>(items.Count);

        foreach (var item in items) {
            var route = new RouteConfig() {
                ClusterId = item.Name,
                RouteId = item.Name,
                Match = new() {
                    Path = $"{item.Name}/{{**remainder}}",
                },
            };
            routes.Add(route.WithTransformPathRemovePrefix($"/{item.Name}"));

            clusters.Add(new() {
                ClusterId = item.Name,
                Destinations = new Dictionary<string, DestinationConfig>() {
                    {
                        $"{item.Name}/destination", new DestinationConfig() {
                            Address = item.Target,
                        }
                    }
                },
                HttpRequest = new ForwarderRequestConfig() {
                    AllowResponseBuffering = false,
                    ActivityTimeout = TimeSpan.FromHours(8),
                }
            });
        }

        return (routes, clusters);
    }
}