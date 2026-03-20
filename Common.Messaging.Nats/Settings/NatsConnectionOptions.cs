using Common.Messaging.Nats.Authorize;

namespace Common.Messaging.Nats.Settings;

/// <summary>
/// Настройки подключения к Nats/
/// </summary>
public sealed class NatsConnectionOptions
{
    /// <summary>
    /// Путь  NATS сервера  nats://localhost:4222).
    /// </summary>
    public required string Broker { get; set; }
    
    /// <summary>
    /// Идентификатор клиента.
    /// </summary>
    public string? ClientId { get; set; }
    
    /// <summary>
    /// Таймаут подключения (секунды).
    /// </summary>
    public int ConnectTimeout { get; set; } = 2;

    /// <summary>
    /// Таймаут пинга (секунды).
    /// </summary>
    public TimeSpan PingInterval { get; set; } = TimeSpan.FromSeconds(10);

    /// <summary>
    /// Максимальное количество пингов без ответа.
    /// </summary>
    public int MaxPingsOut { get; set; } = 2;

    /// <summary>
    /// Таймаут ожидания ответа (секунды).
    /// </summary>
    public int RequestTimeout { get; set; } = 10;

    /// <summary>
    /// Таймаут отключения (секунды).
    /// </summary>
    public int ReconnectWait { get; set; } = 2;

    /// <summary>
    /// Максимальное количество попыток переподключения.
    /// </summary>
    public int MaxReconnect { get; set; } = 60;

    /// <summary>
    /// Включить детальное логирование.
    /// </summary>
    public bool Verbose { get; set; }

    /// <summary>
    /// Включить эхо-режим (получение своих же сообщений).
    /// </summary>
    public bool Echo { get; set; } = true;
    
    /// <summary>
    /// Креденшелы для аутентификации.
    /// </summary>
    public NatsAuthOptions? Auth { get; set; }

}