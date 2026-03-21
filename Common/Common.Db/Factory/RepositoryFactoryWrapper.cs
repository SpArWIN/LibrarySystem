
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Common.Db.Factory;

/// <inheritdoc />
public sealed class RepositoryFactoryWrapper<TDbContext, T> : RepositoryFactoryWrapperBase<TDbContext>
where TDbContext : DbContext
where T : class
{
    private readonly Lazy<Func<TDbContext, T>> _create;
    private readonly Assembly[]? _assemblies;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="assemblies">Список сборок.</param>
    public RepositoryFactoryWrapper(Assembly[] assemblies)
    {
        _assemblies = assemblies ?? throw new ArgumentNullException(nameof(assemblies));
        _create = new Lazy<Func<TDbContext, T>>(CreateFactory);
    }
    
    
    /// <inheritdoc />
    public override object Create(TDbContext dbContext) => _create.Value(dbContext);

    private  Func<TDbContext, T> CreateFactory()
    {
        var implementationType = FindImplementation(typeof(T), _assemblies);
        var constructorInfo = implementationType.GetConstructor(new[] { typeof(TDbContext) });
        
        if (constructorInfo is null)
        {
            throw new InvalidOperationException($"Type '{typeof(TDbContext).FullName}' does not have a parameterless constructor.");
        }
        var contextParameter = Expression.Parameter(typeof(TDbContext));
        var body  = Expression.New(constructorInfo, contextParameter);
        var lamda = Expression.Lambda<Func<TDbContext, T>>(body, contextParameter);
        return lamda.Compile();
    }

    private static Type FindImplementation(Type repositoryInterface, Assembly[] assemblies)
    {
        if (!repositoryInterface.IsInterface)
        {
            throw new InvalidOperationException($"{repositoryInterface.Name} must be an interface.");
        }

        if (assemblies.Length == 0)
        {
            throw new InvalidOperationException($"{assemblies} must have an assembly defined.");
        }
         
        var implementor = assemblies
            .SelectMany(SafeGetTypes)
            .Where(x => x is { IsAbstract: false, IsInterface: false }
                        && repositoryInterface.IsAssignableFrom(x))
            .FirstOrDefault(t => t.GetConstructors().Any(c =>
                c.GetParameters().Any(p => p.ParameterType.IsAssignableFrom(typeof(TDbContext)))));

        if (implementor is null)
        {
            throw new InvalidOperationException(
                $"No implementation of {repositoryInterface.Name} with ctor accepting {typeof(TDbContext).Name} found.");
        }
        return implementor;
    }
    
    private static IEnumerable<Type> SafeGetTypes(Assembly asm)
    {
        try
        {
            return asm.GetTypes();
        }
        catch (ReflectionTypeLoadException ex)
        {
            return ex.Types.Where(t => t is not null)!;
        }
    }
}