using System.Collections.Immutable;

namespace BlazorApp1.ReverseProxy;

public record ConfigFile(ImmutableList<Component> Components);