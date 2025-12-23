using Common.Db.Abstractions;
using Common.Db.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Db.Extensions;

/// <summary>
/// Расширения на <see cref="IUnitOfWork"/>
/// </summary>
public static class UnitOfWorkExtensions
{
    /// <summary>
    /// Добавить Единицу работы.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <typeparam name="TDbContext">Контекст базы данных.</typeparam>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services)
    where TDbContext : DbContext
    {
        services.AddTransient<IUnitOfWork, UnitOfWork<TDbContext>>();
        return services;
    }
}