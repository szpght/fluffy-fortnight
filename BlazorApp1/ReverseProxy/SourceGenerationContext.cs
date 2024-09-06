using System.Text.Json.Serialization;

namespace BlazorApp1.ReverseProxy;

[JsonSourceGenerationOptions(WriteIndented = true, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ConfigFile))]
public partial class SourceGenerationContext : JsonSerializerContext
{
}