using Common.Contracts.Constaints.Providers;
using Common.Contracts.Settings;
using Common.Db.Factory;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Core.Infrastructure.Context;

/// <inheritdoc />
public sealed class LibraryDbContextFactory : IAppDbContextFactory<LibraryDbContext>
{
   private readonly IOptions<LibraryDbContextFactoryOptions> _options;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="baseOptions"><see cref="DbContextOptions"/>.</param>
    public LibraryDbContextFactory(IOptions<LibraryDbContextFactoryOptions> baseOptions)
    {
        _options = baseOptions;
    }

    /// <inheritdoc />
    public LibraryDbContext Create(DataBaseSettings dbSettings)
    {
        var builder = new DbContextOptionsBuilder<LibraryDbContext>();
        var migrationsAssembly = string.IsNullOrWhiteSpace(dbSettings.MigrationsAssembly)
            ? typeof(LibraryDbContext).Assembly.GetName().Name
            : dbSettings.MigrationsAssembly;
        
        ApplyBaseOptions(builder, _options.Value);
        switch (dbSettings.Provider.ToLowerInvariant())
        {
            case Provider.Postgres:
            {
                builder.UseNpgsql(dbSettings.ConnectionString, opt => opt.MigrationsAssembly(migrationsAssembly));
                break;
            }
            default:
                throw new InvalidOperationException($" Unsupported provider '{dbSettings.Provider}'.");
        }
        return new LibraryDbContext(builder.Options);
    }
    
    private static void ApplyBaseOptions(DbContextOptionsBuilder builder, LibraryDbContextFactoryOptions opt)
    {
        builder.EnableDetailedErrors(opt.EnableDetailedErrors);
        if (opt.EnableSensitiveDataLogging)
            builder.EnableSensitiveDataLogging();
    }
}