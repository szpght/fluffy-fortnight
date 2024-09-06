using System.Text.Json.Serialization;

namespace BlazorApp1.ReverseProxy;

public record Component(
    [property: JsonPropertyOrder(1)] string Name,
    [property: JsonPropertyOrder(2)] string LocalTarget,
    [property: JsonPropertyOrder(3)] string RemoteTarget,
    [property: JsonPropertyOrder(4)] bool IsLocal) {
    [JsonIgnore] public string Target => IsLocal ? LocalTarget : RemoteTarget;
};