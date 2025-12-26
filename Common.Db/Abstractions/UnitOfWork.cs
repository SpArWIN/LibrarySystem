using Common.Db.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Common.Db.Abstractions;

/// <inheritdoc />
public sealed class UnitOfWork<TDbContext> : IUnitOfWork
where TDbContext : DbContext
{
    private readonly IRepositoryFactory<TDbContext> _repoFactory;
    private IDbContextTransaction? _transaction;

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
        Context = context  ?? throw new ArgumentNullException(nameof(context));;
    }
    
    
    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is null)
        {
            return;
        }
        
        await Context.SaveChangesAsync(cancellationToken);
        await _transaction.CommitAsync(cancellationToken);
        await DisposeTransactionAsync();
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
    
    private async Task DisposeTransactionAsync()
    {
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        if (Context is IAsyncDisposable asyncDisposable)
        {
            await asyncDisposable.DisposeAsync();
        }
        else
        {
            await Context.DisposeAsync();
        }
    }
}