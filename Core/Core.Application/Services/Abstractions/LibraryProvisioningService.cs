using Common.Contracts.Database;
using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Core.Domain.Models.Instanse;
using Core.Infrastructure.Extensions.UnitOfWorks;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.Configuration;
using Serilog;

namespace Core.Application.Services.Abstractions;

/// <inheritdoc />
public sealed class LibraryProvisioningService(
    IDatabaseProvisioner databaseProvisioner, 
    IOptions<TenantProvisioningOptions> options) : ILibraryProvisioningService

{
    private static readonly ILogger Logger = Log.ForContext<LibraryProvisioningService>();
    
    /// <inheritdoc />
    public async Task<CreateLibraryResponseDto> CreateAsync(IUnitOfWork centralUow, 
        CreateLibraryRequestDto request, 
        CancellationToken ct = default)
    {
        Logger.Information("-> Process create library {Name}", request.Name);
        ArgumentNullException.ThrowIfNull(centralUow);
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrEmpty(request.Name))
        {
            throw new InvalidOperationException("Library name is empty.");
        }
        var repository = centralUow.GetLibraryInstanceRepository();

        if (await repository.ExistsByNameAsync(name: request.Name, ct))
        {
            throw new InvalidOperationException($"Library '{request.Name}' already exists.");
        }
        
        var now = DateTimeOffset.UtcNow;
        var libraryId = Guid.NewGuid();
        var dbName = $"{options.Value.DatabaseNamePrefix}{libraryId:N}";
        await databaseProvisioner.CreateDatabaseAsync(dbName, ct);
        var connectionString = options.Value.TenantConnectionStringTemplate.Replace("{db}", dbName, StringComparison.Ordinal);


        if (await databaseProvisioner.DatabaseExistsAsync(dbName, ct))
        {
            throw new InvalidOperationException($"Database '{dbName}' already exists.");
        }

        try
        {
            await repository.AddAsync(new LibraryInstance()
            {
                Id = libraryId,
                Name = request.Name.Trim(),
                ConnectionString = connectionString,
                MigrationsAssembly = options.Value.MigrationsAssembly,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            },ct);
            Logger.Information("<- Process create library {name} success", request.Name);
            return new CreateLibraryResponseDto{LibraryId = libraryId};
        }
        catch
        {
            await databaseProvisioner.DropDatabaseAsync(dbName, ct);
            throw;
        }
        
    }

    /// <inheritdoc />
    public async Task DeleteAsync(IUnitOfWork centralUow, Guid libraryId, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(centralUow);
        var repository = centralUow.GetLibraryInstanceRepository();
        var instance = await repository.FindAsync(libraryId, ct)
            ?? throw new NullReferenceException($"Instance with id {libraryId} not found");
        var dbName = ExtractDbNameFromConnectionString(instance.ConnectionString);
        await databaseProvisioner.DropDatabaseAsync(dbName, ct);
        await repository.RemovesAsync([instance], ct);
    }
    
    private static string ExtractDbNameFromConnectionString(string connectionString)
    {
        //TODO потом можно и нужно будет сделать мультивыбор.
        var builder = new Npgsql.NpgsqlConnectionStringBuilder(connectionString);
        return builder?.Database;
    }
}