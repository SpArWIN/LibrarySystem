using Microsoft.EntityFrameworkCore;

namespace Common.Db.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    /// <summary>
    /// Контекст базы данных.
    /// </summary>
    DbContext Context { get; }
    
    /// <summary>
    /// Получить репозиторий.
    /// </summary>
    /// <typeparam name="T">Тип репозитория.</typeparam>
    /// <returns>T - тип репозитория.</returns>
    T GetRepository<T>()
    where T : class;
    
    /// <summary>
    /// Применить изменения.
    /// </summary>
    /// <param name="cancellationToken">.</param>
    /// <returns>.</returns>
    Task CommitAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Отменить изменения.
    /// </summary>
    /// <param name="cancellationToken">.</param>
    /// <returns>.</returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);
    
    /// <summary>Начать транзакцию, если не начата.</summary>
    Task BeginTransactionAsync(CancellationToken ct = default);
    
    /// <summary>Активна ли транзакция.</summary>
    bool HasActiveTransaction { get; }
}