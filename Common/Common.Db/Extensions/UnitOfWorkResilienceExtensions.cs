using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Db.Factory;
using Microsoft.EntityFrameworkCore;
using Polly;

namespace Common.Db.Extensions;

/// <summary>
/// Расширение, с ретрями на <see cref="IUnitOfWork"/>.
/// </summary>
public static class UnitOfWorkResilienceExtensions
{
    /// <summary>
    /// Начать транзакцию, создавая попытки.
    /// </summary>
    /// <param name="factory"><see cref="IUnitOfWorkFactory{TDbContext}"/>.</param>
    /// <param name="action">Выполняемое действие.</param>
    /// <param name="pipeline"><see cref="ResiliencePipeline"/> Политики повтора попыток.</param>
    /// <param name="settings">Настройки подключения к конкретной базе, если необходимы.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="TResult">Тип результата.</typeparam>
    /// <typeparam name="TDbContext">Контекст базы данных.</typeparam>
    /// <returns>Результат выполнения действия.</returns>
    public static ValueTask<TResult> CreateWithRetryAsync<TDbContext,TResult>
    (this IUnitOfWorkFactory<TDbContext> factory,
        Func<IUnitOfWork, Task<TResult>> action,
        ResiliencePipeline pipeline,
        CancellationToken cancellationToken = default,
        DataBaseSettings? settings = null
        )
        where TDbContext : DbContext
        => pipeline.ExecuteAsync(async token =>
     {
         await using var uow = await factory.CreateAsync(beginTransaction: true, token, settings);
         try
         {
             var result = await action(uow);
             await uow.CommitAsync(token);
             return result;
         }
         catch (OperationCanceledException)
         {
             await uow.RollbackAsync(cancellationToken);
             throw;
         }
         catch
         {
             await uow.RollbackAsync(cancellationToken);
             throw;
         }
     }, cancellationToken);

    /// <summary>
    /// Выполнить действие для чтения, вне транзакций.
    /// </summary>
    /// <param name="factory"><see cref="IUnitOfWorkFactory{TDbContext}"/>.</param>
    /// <param name="action">Выполняемое действие.</param>
    /// <param name="pipeline"><see cref="ResiliencePipeline"/> Политики повтора попыток.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <param name="settings">Настройки подключения к базе, если это необходимо.</param>
    /// <typeparam name="TResult">Тип результата.</typeparam>
    /// <typeparam name="TDbContext">Контекст базы данных.</typeparam>
    /// <returns></returns>
    public static ValueTask<TResult> ExecuteQueryAsync<TDbContext,TResult>(
        this IUnitOfWorkFactory<TDbContext> factory,
        Func<IUnitOfWork, Task<TResult>> action,
        ResiliencePipeline pipeline,
        CancellationToken ct = default,
        DataBaseSettings? settings = null
        )
    where TDbContext : DbContext
    {
        return pipeline.ExecuteAsync(async token =>
        {
           var uow = await factory.CreateAsync(beginTransaction: false, token, settings);
            try
            {
                return await action(uow);
            }
            finally
            {
                await uow.DisposeAsync();
            }
        }, ct);
    }
}