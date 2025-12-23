using Common.Db.Abstractions;

namespace Common.Db.Factory;

/// <summary>
/// Фабрика <see cref="IUnitOfWork"/>
/// </summary>
public interface IUnitOfWorkFactory
{
    /// <summary>
    /// Создать UnitOfWork.
    /// </summary>
    /// <param name="beginTransaction">Применить ли транзакцию.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns><see cref="IUnitOfWork"/>.</returns>
    Task<IUnitOfWork> CreateAsync(bool beginTransaction = false ,CancellationToken ct = default);
}