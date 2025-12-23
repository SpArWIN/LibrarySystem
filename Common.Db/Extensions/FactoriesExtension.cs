using System.Reflection;
using Common.Db.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Db.Extensions;

/// <summary>
/// Расширение для фабрик.
/// </summary>
public static class FactoriesExtension
{
    /// <summary>
    /// Подключить DbContext и фабрики репозиториев.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="assemblies">Сборки для формирования получения репозиториев.</param>
    /// <param name="dbOptions"><see cref="DbContextOptionsBuilder"/>.</param>
    /// <typeparam name="TDbContext">Тип контекста.</typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCommonDb<TDbContext>(this IServiceCollection services,
        Assembly[] assemblies,
        Action<DbContextOptionsBuilder>? dbOptions)
        where TDbContext : DbContext
    {
        services.AddDbContext<TDbContext>(dbOptions);
        services.AddRepositoryFactory<TDbContext>(assemblies);
        services.AddSingleton<IUnitOfWorkFactory>(sp => new UnitOfWorkFactory<TDbContext>(sp.GetRequiredService<IServiceScopeFactory>()));
        services.AddUnitOfWork<TDbContext>();
        return services;
    }
    
    //services.AddRepositoryFactory<LibraryDbContext>();
    /// <summary>
    /// Добавить фабрику репозиториев.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="scanAssemblies">Сборки на репозитории.</param>
    /// <typeparam name="TDbContext"></typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    private static IServiceCollection AddRepositoryFactory<TDbContext>(
        this IServiceCollection services, 
        params Assembly [] scanAssemblies)
    where TDbContext : DbContext
    {
      return  services.AddScoped<IRepositoryFactory<TDbContext>>(_ => new RepositoryFactory<TDbContext>(scanAssemblies));
    }

    
    
}