using Core.Infrastructure.Extensions.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Core.Infrastructure.Factory;

/// <inheritdoc />
public sealed class CentralDbContextDesignTimeFactory : IDesignTimeDbContextFactory<CentralDbContext>
{
    /// <inheritdoc />
    public CentralDbContext CreateDbContext(string[] args)
    {
        var cfg = BuildConfiguration();
        var dbSettings = cfg.GetSettings();
        var options = new DbContextOptionsBuilder<CentralDbContext>();
        DbRegistration.ConfigureDbContext(options, dbSettings);
        return new CentralDbContext(options.Options);
    }

    public static IConfiguration BuildConfiguration()
    {
        var env = Environment.GetEnvironmentVariable("Development") ?? "Production";
        
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables(prefix: "LIBRARY__")
            .Build();
    }
}