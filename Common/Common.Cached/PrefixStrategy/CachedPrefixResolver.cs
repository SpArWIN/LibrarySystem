namespace Common.Cached.PrefixStrategy;

/// <inheritdoc />
public sealed class CachedPrefixResolver : ICachedResolver
{
    private readonly Dictionary<Type, ICachePrefixStrategy> _strategies;
    
    
    public CachedPrefixResolver(IEnumerable<ICachePrefixStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s=> s.TargetType);
    }


    /// <inheritdoc />
    public string GetPrefix<T>() where T : class
    {
        return _strategies.TryGetValue(typeof(T), out var strategy) 
            ? strategy.GetPrefix() : 
            $"{typeof(T).Name.ToLowerInvariant()}";
    }
}