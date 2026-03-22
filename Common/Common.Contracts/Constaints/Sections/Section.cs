namespace Common.Contracts.Constaints.Sections;

/// <summary>
/// Константы секций.
/// </summary>
public static class Section
{
    /// <summary>
    /// Секция базы данных.
    /// </summary>
    public const string Database = nameof(Database);
    
    /// <summary>
    /// Секция токена JWT.
    /// </summary>
    public const string Jwt = nameof(Jwt);
    
    /// <summary>
    /// Секция RefreshToken.
    /// </summary>
    public const string RefreshToken = nameof(RefreshToken);
    
    /// <summary>
    /// Настройки центральной базы данных.
    /// </summary>
    public const string TenantDatabase = nameof(TenantDatabase);
    
    /// <summary>
    /// Настройки шаблонов дополнительных баз данных.
    /// </summary>
    public const string TenantProvisioning = nameof(TenantProvisioning);
    
    /// <summary>
    /// Настройки базовой установки ноовго контекста дополнительной базы данных.
    /// </summary>
    public const string LibraryDbContextFactory = nameof(LibraryDbContextFactory);
    
    /// <summary>
    /// Секция Nats.
    /// </summary>
    public const string Nats = nameof(Nats);
    
    /// <summary>
    /// Секция JetStream
    /// </summary>
    public const string JetStream = nameof(JetStream);
    
    /// <summary>
    /// Секция для настроек логгов.
    /// </summary>
    public const string Logging = nameof(Logging);
}