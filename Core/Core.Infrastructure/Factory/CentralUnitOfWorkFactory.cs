using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Db.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Factory;

/// <summary>
/// Фабрика для основного контекста базы данных.
/// </summary>
public sealed class CentralUnitOfWorkFactory(
    IServiceScopeFactory serviceScopeFactory) : IUnitOfWorkFactory<CentralDbContext>

{
    /// <inheritdoc />
    public async Task<IUnitOfWork> CreateAsync(
        bool beginTransaction = false,
        CancellationToken ct = default,
        DataBaseSettings? dbSettings = null)
    {
        var scope = serviceScopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<CentralDbContext>();
        var repoFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory<CentralDbContext>>();

        var uow = new UnitOfWork<CentralDbContext>(repoFactory, context);

        if (beginTransaction)
        {
            await uow.BeginTransactionAsync(ct);
        }

        return uow;
    }
}