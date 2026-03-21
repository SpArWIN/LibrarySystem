namespace Common.Messaging.Nats.Authorize;

/// <summary>
/// Авторизация в Nats.
/// </summary>
public sealed class NatsAuthOptions
{
    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Пароль.
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// JWT токен.
    /// </summary>
    public string? Jwt { get; set; }

    /// <summary>
    /// Секретный ключ (NKey).
    /// </summary>
    public string? NKey { get; set; }

    /// <summary>
    /// Файл с учетными данными.
    /// </summary>
    public string? CredsFile { get; set; }

    /// <summary>
    /// Токен.
    /// </summary>
    public string? Token { get; set; }
}