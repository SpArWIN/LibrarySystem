using Common.Db.Abstractions;
using Common.Db.Factory;
using Common.Policies.PipelineNames;
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
    /// <param name="factory"><see cref="IUnitOfWorkFactory"/>.</param>
    /// <param name="action">Выполняемое действие.</param>
    /// <param name="pipeline"><see cref="ResiliencePipeline"/> Политики повтора попыток.</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="TResult">Тип результата.</typeparam>
    /// <returns>Результат выполнения действия.</returns>
    public static ValueTask<TResult> CreateWithRetryAsync<TResult>(
        this IUnitOfWorkFactory factory,
        Func<IUnitOfWork, Task<TResult>> action,
        ResiliencePipeline pipeline,
        CancellationToken cancellationToken = default) => pipeline.ExecuteAsync(async token =>
     {
         await using var uow = await factory.CreateAsync(beginTransaction: true, token);
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
    /// <param name="factory"><see cref="IUnitOfWorkFactory"/>.</param>
    /// <param name="action">Выполняемое действие.</param>
    /// <param name="pipeline"><see cref="ResiliencePipeline"/> Политики повтора попыток.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <typeparam name="TResult">Тип результата.</typeparam>
    /// <returns></returns>
    public static ValueTask<TResult> ExecuteQueryAsync<TResult>(
        this IUnitOfWorkFactory factory,
        Func<IUnitOfWork, Task<TResult>> action,
        ResiliencePipeline pipeline,
        CancellationToken ct = default)
    {
        return pipeline.ExecuteAsync(async token =>
        {
           var uow = await factory.CreateAsync(beginTransaction: false, token);
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