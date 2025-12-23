using Common.Db.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Db.Factory;

/// <inheritdoc />
public sealed class UnitOfWorkFactory<TDbContext> : IUnitOfWorkFactory
where TDbContext : DbContext
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="serviceScopeFactory"><see cref="IServiceScopeFactory"/>.</param>
    public UnitOfWorkFactory(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
    }
    
    /// <inheritdoc />
    public async Task<IUnitOfWork> CreateAsync(bool beginTransaction = false, CancellationToken ct = default)
    {
      var scope = _serviceScopeFactory.CreateScope();
      var repoFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory<TDbContext>>();
      var uow = new UnitOfWork<TDbContext>(repoFactory, scope);
      if (beginTransaction)
      {
          await uow.BeginTransactionAsync(ct);
      }
      return uow;
    }
}