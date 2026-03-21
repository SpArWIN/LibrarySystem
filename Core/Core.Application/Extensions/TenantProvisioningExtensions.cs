using Common.Contracts.Constaints.Sections;
using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Core.Application.Services.Abstractions;
using Core.Infrastructure.DbProvisioning;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Extensions;

/// <summary>
/// Расширение на добавление сервисов по созданию баз данных.
/// </summary>
public static class TenantProvisioningExtensions
{
    /// <summary>
    /// Добавить конфигурации сервисов по созданию баз данных.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddTenantProvisioningServices(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.Configure<TenantProvisioningOptions>(configuration.GetSection(Section.TenantProvisioning));
        services.AddSingleton<IDatabaseProvisioner, PostgresDatabaseProvisioner>();
        services.AddSingleton<ITenantDatabaseMigrator, TenantDatabaseMigrator>();
        services.AddScoped<ILibraryProvisioningService, LibraryProvisioningService>();
        return services;
    }
}