using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Db.Factory;
using Common.Http.Accessors;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Factory;

/// <inheritdoc />
public sealed class UnitOfWorkFactory : IUnitOfWorkFactory<LibraryDbContext>

{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly IAppDbContextFactory<LibraryDbContext> _appDbContextFactory;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="serviceScopeFactory"><see cref="IServiceScopeFactory"/>.</param>
    /// <param name="appDbContextFactory"><see cref="IAppDbContextFactory{TDbContext}"/>.</param>
    public UnitOfWorkFactory(IServiceScopeFactory serviceScopeFactory, IAppDbContextFactory<LibraryDbContext> appDbContextFactory)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _appDbContextFactory = appDbContextFactory;
    }
    
    /// <inheritdoc />
    public async Task<IUnitOfWork> CreateAsync(bool beginTransaction = false, 
        CancellationToken ct = default,
        DataBaseSettings? dbSettings = null)
    {
        var scope = _serviceScopeFactory.CreateScope();
        try
        {
            dbSettings ??= scope.ServiceProvider
                               .GetRequiredService<ITenantContextAccessor>()
                               .CurrentContext?.DbSettings
                           ?? throw new InvalidOperationException("Database settings are not resolved.");
            
            var context = _appDbContextFactory.Create(dbSettings);
            
            var repoFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory<LibraryDbContext>>();
            var uow = new UnitOfWork<LibraryDbContext>(repoFactory, context);
            if (beginTransaction)
            {
                await uow.BeginTransactionAsync(ct);
            }

            return uow;
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }
}