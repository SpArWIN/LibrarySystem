using Common.Contracts.Constaints.Sections;
using Common.Contracts.Settings;
using Core.Infrastructure.Context;
using Core.Infrastructure.Extensions.DataBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
namespace Core.Infrastructure.Factory;

/// <inheritdoc />
public sealed class LibraryDbContextDesignTimeFactory : IDesignTimeDbContextFactory<LibraryDbContext>
{
    /// <inheritdoc />
    public LibraryDbContext CreateDbContext(string[] args)
    {
        var cfg = CentralDbContextDesignTimeFactory.BuildConfiguration();
        var provisioning = cfg.GetSection(Section.TenantProvisioning).Get<TenantProvisioningOptions>()
            ?? throw new ArgumentNullException(Section.TenantProvisioning);

        if (string.IsNullOrWhiteSpace(provisioning.Provider) ||
            string.IsNullOrWhiteSpace(provisioning.TenantConnectionStringTemplate))
        {
            throw new ArgumentNullException(Section.TenantProvisioning);
        }

        var dbSettings = new DataBaseSettings()
        {
            Provider = provisioning.Provider,
            ConnectionString =
                provisioning.TenantConnectionStringTemplate.Replace("{db]", "library_template",
                    StringComparison.Ordinal),
            MigrationsAssembly = string.IsNullOrWhiteSpace(provisioning.MigrationsAssembly)
                ? typeof(LibraryDbContext).Assembly.GetName().Name
                : provisioning.MigrationsAssembly
        };
        var options = new DbContextOptionsBuilder<LibraryDbContext>();
        DbRegistration.ConfigureDbContext(options, dbSettings);
        return new LibraryDbContext(options.Options);
    }

  
}