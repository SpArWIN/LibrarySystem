using Common.Contracts.Settings;
using Microsoft.EntityFrameworkCore;

namespace Common.Db.Factory;

/// <summary>
/// Фабрика построения контекста баз данных.
/// </summary>
/// <typeparam name="TDbContext"></typeparam>
public interface IAppDbContextFactory<out TDbContext>
where TDbContext : DbContext
{
    /// <summary>
    /// Построить контекст базы данных.
    /// </summary>
    /// <param name="dbSettings"><see cref="DataBaseSettings"/>.</param>
    /// <returns></returns>
    TDbContext Create(DataBaseSettings dbSettings);
}