namespace Common.Contracts.Storage.Settings;

/// <summary>
/// Настройки подключения к MINIO.
/// </summary>
public sealed class MinioOptions
{
    /// <summary>
    /// Точка обращения.
    /// </summary>
    public required string Endpoint { get; init; } 
    
    /// <summary>
    /// Токен.
    /// </summary>
    public required string AccessKey { get; init; }
    
    /// <summary>
    /// Ключ.
    /// </summary>
    public required string SecretKey { get; init; }
    
    /// <summary>
    /// Порт.
    /// </summary>
    public int Port { get; init; }
    
    /// <summary>
    /// Подключать SSl.
    /// </summary>
    public bool WithSsl { get; init; }

    /// <summary>
    /// Публичный хост для выдачи пресайн‑URL (доступен из браузера). Если не задан, используется Endpoint.
    /// </summary>
    public string? PublicEndpoint { get; init; }

    /// <summary>
    /// Публичный порт для пресайн‑URL. Если 0, порт опускается.
    /// </summary>
    public int PublicPort { get; init; }
}