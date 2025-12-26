using Common.Contracts.Constaints.Sections;
using Common.Contracts.Settings;
using Common.Db.Factory;
using Core.Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions.Context;

/// <summary>
/// Расширение на <see cref="LibraryDbContextFactory"/>
/// </summary>
public static class ContextConfigurationExtensions
{
    /// <summary>
    /// Добавить контекст конфигурации построения баз данных.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddContextConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LibraryDbContextFactoryOptions>(configuration.GetSection(Section.LibraryDbContextFactory));
        services.AddSingleton<IAppDbContextFactory<LibraryDbContext>, LibraryDbContextFactory>();
        return services;
    }
}