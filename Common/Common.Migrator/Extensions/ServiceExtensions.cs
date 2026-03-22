using Common.Contracts.Constaints.Sections;
using Common.Contracts.Settings;
using Common.Db.Factory;
using Common.Migrator.Services;
using Core.Infrastructure.Context;
using Core.Infrastructure.Extensions.DataBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Migrator.Extensions;

public static class ServiceExtensions
{
    /// <summary>
    /// Добавить сервис миграций.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns></returns>
    public static IServiceCollection AddMigrator(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCentralDbContext(configuration);
        services.Configure<TenantDatabaseOptions>(configuration.GetSection(Section.TenantDatabase));
        services.AddSingleton<IAppDbContextFactory<LibraryDbContext>, LibraryDbContextFactory>();
        services.AddSingleton<IDatabaseMigrator, DatabaseMigrator>();
        return services;
    }
}