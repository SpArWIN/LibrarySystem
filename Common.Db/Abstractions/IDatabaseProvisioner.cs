namespace Common.Db.Abstractions;

/// <summary>
/// Сервис создания физической базы данных.
/// </summary>
public interface IDatabaseProvisioner
{
    /// <summary>
    /// Проверить сущесствование бд.
    /// </summary>
    /// <param name="databaseName">Имя базы данных.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    /// <returns>True/False.</returns>
    Task<bool> DatabaseExistsAsync(string databaseName, CancellationToken ct = default);
    
    /// <summary>
    /// Создать базу.
    /// </summary>
    /// <param name="databaseName">Название базы данных.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task CreateDatabaseAsync(string databaseName, CancellationToken ct = default);
    
    /// <summary>
    /// Удалить базу.
    /// </summary>
    /// <param name="databaseName">Имя базы.</param>
    /// <param name="ct"><see cref="CancellationToken"/>.</param>
    Task DropDatabaseAsync(string databaseName, CancellationToken ct = default);
}