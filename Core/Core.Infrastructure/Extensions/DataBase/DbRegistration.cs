using Common.Contracts.Constaints.Providers;
using Common.Contracts.Constaints.Sections;
using Common.Contracts.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Infrastructure.Extensions.DataBase;

/// <summary>
///  Расширение на регистрацию контекста базы данных.
/// </summary>
public static class DbRegistration
{
    /// <summary>
    /// Добавить централизированный контекст базы данных.
    /// </summary>
    /// <param name="serviceCollection"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddCentralDbContext(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    { 
        serviceCollection.AddDbContext<CentralDbContext>(options =>
            ConfigureDbContext(options, configuration.BindDataBaseSettings()));
        return serviceCollection;
    }
    

    private static DataBaseSettings BindDataBaseSettings(this IConfiguration configuration)
    {
        var configurations = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddEnvironmentVariables("LIBRARY__")
            .Build();

        var settings = configurations.GetSettings();
       
        return settings;
    }

    /// <summary>
    /// Получить настройки.
    /// </summary>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns>.</returns>
    public static DataBaseSettings GetSettings(this IConfiguration configuration)
    {
        var section = configuration.GetSection(Section.Database);
        var settings = section.Get<DataBaseSettings>();
        if (settings is null || string.IsNullOrWhiteSpace(settings.Provider)
                             || string.IsNullOrWhiteSpace(settings.ConnectionString))
            throw new ApplicationException("Database settings are missing or invalid.");
        return settings;
    }

    public static void ConfigureDbContext(DbContextOptionsBuilder options, DataBaseSettings dbSettings)
    {
        var migrations = string.IsNullOrWhiteSpace(dbSettings.MigrationsAssembly)
            ? typeof(CentralDbContext).Assembly.GetName().Name
            : dbSettings.MigrationsAssembly;

        switch (dbSettings.Provider.ToLowerInvariant())
        {
            case Provider.Postgres:
            {
                options.UseNpgsql(dbSettings.ConnectionString,
                    opt => opt.MigrationsAssembly(migrations));
                break;
            }
            default:
                throw new InvalidOperationException($" Unsupported provider '{dbSettings.Provider}'.");
        }
    }
}