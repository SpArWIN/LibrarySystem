using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;

namespace Core.Infrastructure.DbProvisioning;

/// <inheritdoc />
public sealed class PostgresDatabaseProvisioner(IOptions<TenantProvisioningOptions> options) : IDatabaseProvisioner
{
    /// <inheritdoc />
    public async Task<bool> DatabaseExistsAsync(string databaseName, CancellationToken ct = default)
    {
        await using var conn = new NpgsqlConnection(options.Value.AdminConnectionString);
        await conn.OpenAsync(ct);

        const string sql = "SELECT 1 FROM pg_database WHERE datname = @db;";
        await using var cmd = new NpgsqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("db", databaseName);

        var result = await cmd.ExecuteScalarAsync(ct);
        return result is not null;
    }

    
    /// <inheritdoc />
    public async Task CreateDatabaseAsync(string databaseName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name is empty.", nameof(databaseName));
        }
        await using var conn = new NpgsqlConnection(options.Value.AdminConnectionString);
        await conn.OpenAsync(ct);
        var sql = $"CREATE DATABASE \"{databaseName}\"";
        await using var cmd = new NpgsqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync(ct);
        await conn.CloseAsync();
    }

    
    /// <inheritdoc />
    public async Task DropDatabaseAsync(string databaseName, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(databaseName))
        {
            throw new ArgumentException("Database name is empty.", nameof(databaseName));
        }
        await using var conn = new NpgsqlConnection(options.Value.AdminConnectionString);
        await conn.OpenAsync(ct);
        var terminateSql = """
           SELECT pg_terminate_backend(pid)
           FROM pg_stat_activity
           WHERE datname = @db AND pid <> pg_backend_pid();
       """;

        await using var terminate = new NpgsqlCommand(terminateSql, conn);
        terminate.Parameters.AddWithValue("db", databaseName);
        await terminate.ExecuteNonQueryAsync(ct);
        
        var dropSql = $"DROP DATABASE IF EXISTS \"{databaseName}\"";
        await using var drop = new NpgsqlCommand(dropSql, conn);
        await drop.ExecuteNonQueryAsync(ct);
    }
}