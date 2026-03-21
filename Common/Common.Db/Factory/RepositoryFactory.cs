using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Common.Db.Factory;

/// <inheritdoc />
public sealed class RepositoryFactory<TDbContext> : IRepositoryFactory<TDbContext>
where TDbContext : DbContext
{
    private static readonly ConcurrentDictionary<Type, RepositoryFactoryWrapperBase<TDbContext>> _factoryWrappers = new();
    private readonly Assembly[] _assemblies;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="assemblies"><see cref="Assembly"/>.</param>
    public RepositoryFactory(params Assembly [] assemblies)
    {
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
    }
    
    /// <inheritdoc />
    public T Create<T>(TDbContext dbContext) where T : class
    {
        var factory = _factoryWrappers.GetOrAdd(typeof(T), 
            new RepositoryFactoryWrapper<TDbContext, T>(_assemblies));
        return (T)factory.Create(dbContext);
    }
}