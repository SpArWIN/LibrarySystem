using Core.Infrastructure.Constaints.Providers;
using Core.Infrastructure.Constaints.Sections;
using Core.Infrastructure.Context;
using Core.Infrastructure.Settings;
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
    /// Добавить контекст базы данных.
    /// </summary>
    /// <param name="serviceCollection"><see cref="IServiceCollection"/>.</param>
    /// <param name="configuration"><see cref="IConfiguration"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddLibraryDbContext(this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        var db = BindDataBaseSettings(configuration);

        serviceCollection.AddDbContext<LibraryDbContext>(options => ConfigureDbContext(options, db));

        return serviceCollection;
    }

    private static DataBaseSettings BindDataBaseSettings(IConfiguration configuration)
    {
        var configurations = new ConfigurationBuilder()
            .AddConfiguration(configuration)
            .AddEnvironmentVariables("LIBRARY__")
            .Build();

        var section = configurations.GetSection(Section.Database);
        var settings = section.Get<DataBaseSettings>();
        if (settings is null || string.IsNullOrWhiteSpace(settings.Provider)
                             || string.IsNullOrWhiteSpace(settings.ConnectionString))
            throw new ApplicationException("Database settings are missing or invalid.");
        
       
        return settings;
    }

    private static void ConfigureDbContext(DbContextOptionsBuilder options, DataBaseSettings dbSettings)
    {
        var migrations = string.IsNullOrWhiteSpace(dbSettings.MigrationsAssembly)
            ? typeof(DataBaseSettings).Assembly.GetName().Name
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