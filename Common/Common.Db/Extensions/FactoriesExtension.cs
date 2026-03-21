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
    /// <typeparam name="TDbContext">Тип контекста.</typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCommonDb<TDbContext>(this IServiceCollection services,
        Assembly[] assemblies)
        where TDbContext : DbContext
    {
        services.AddRepositoryFactory<TDbContext>(assemblies);
        return services;
    }
    
    
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