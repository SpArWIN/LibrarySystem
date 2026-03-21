using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Common.Db.Factory;

/// <summary>
/// Фабрика <see cref="IUnitOfWork"/>
/// </summary>
public interface IUnitOfWorkFactory<out TDbContext>
where TDbContext : DbContext
{
    /// <summary>
    /// Создать UnitOfWork.
    /// </summary>
    /// <param name="beginTransaction">Применить ли транзакцию.</param>
    /// <param name="dbSettings">Настройки, если не заданы, контекст берется из DI.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="IUnitOfWork"/>.</returns>
    Task<IUnitOfWork> CreateAsync(bool beginTransaction = false ,
        CancellationToken ct = default, 
        DataBaseSettings? dbSettings = null);
    
}