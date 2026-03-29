using System.Collections.Concurrent;
using Common.Db.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Common.Db.Abstractions;

/// <inheritdoc />
public sealed class UnitOfWork<TDbContext> : IUnitOfWork
where TDbContext : DbContext
{
    private readonly IRepositoryFactory<TDbContext> _repoFactory;
    private IDbContextTransaction? _transaction;
    private readonly Lazy<ConcurrentQueue<Func<IUnitOfWork, CancellationToken, Task>>> _preCommitActions;
    private readonly Lazy<ConcurrentQueue<Func<IUnitOfWork, CancellationToken, Task>>> _postCommitActions;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="repoFactory">Фабрика репозиториев.</param>
    /// <param name="context">Необходимый контекст базы данных.</param>
    public UnitOfWork(
        IRepositoryFactory<TDbContext> repoFactory,
        TDbContext context)
    {
        _repoFactory = repoFactory;
        Context = context  ?? throw new ArgumentNullException(nameof(context));

        _preCommitActions = new Lazy<ConcurrentQueue<Func<IUnitOfWork, CancellationToken, Task>>>(
            LazyThreadSafetyMode.ExecutionAndPublication);
        _postCommitActions = new Lazy<ConcurrentQueue<Func<IUnitOfWork, CancellationToken, Task>>>(
            () => [], LazyThreadSafetyMode.ExecutionAndPublication);
    }
    
    
    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            await Context.SaveChangesAsync(cancellationToken);
            return;
        }

        try
        {
            await ExecutePreCommitAsync(cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
            await ExecutePostCommitAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            await RollbackAsync(cancellationToken);
            throw ex;
        }
        finally
        {
            await DisposeTransactionAsync();
        }
        
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return;
        }
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await DisposeTransactionAsync();
        }
    }

    /// <inheritdoc />
    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction ??= await Context.Database.BeginTransactionAsync(ct);
    }
    
    /// <inheritdoc />
    public T GetRepository<T>() where T : class
     => _repoFactory.Create<T>((TDbContext)Context);
    
    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeTransactionAsync();
    }

    /// <inheritdoc />
    public DbContext Context { get; }

    /// <inheritdoc />
    public bool HasActiveTransaction  => _transaction is not null;

    /// <inheritdoc />
    public void AddPreCommit(Func<IUnitOfWork, CancellationToken, Task> action)
    {
       PreCommitActions?.Enqueue(action);
    }

    /// <inheritdoc />
    public void AddPostCommit(Func<IUnitOfWork, CancellationToken, Task> action)
    {
        PostCommitActions?.Enqueue(action);
    }

    private async Task ExecutePreCommitAsync(CancellationToken ct = default)
    {
        if (!_postCommitActions.IsValueCreated)
        {
            return;
        }
        var queue = _postCommitActions.Value;
        while (queue.TryDequeue(out var action))
        {
            await action(this, ct);
        }
    }

    private async Task ExecutePostCommitAsync(CancellationToken ct = default)
    {
        if (!_postCommitActions.IsValueCreated)
        {
          return;
        }
        var queue = _postCommitActions.Value;
        while (queue.TryDequeue(out var action ))
        {
            await action(this, ct);
        }
    }

    private ConcurrentQueue<Func<IUnitOfWork, CancellationToken, Task>>? PreCommitActions => _preCommitActions?.Value;
    private ConcurrentQueue<Func<IUnitOfWork, CancellationToken, Task>>? PostCommitActions => _postCommitActions?.Value;
    
    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }
}