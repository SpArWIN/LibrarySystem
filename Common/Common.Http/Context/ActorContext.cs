namespace Common.Http.Context;

/// <summary>
/// Контекст хранилища Jwt токена.
/// </summary>
public sealed class ActorContext
{
    private static readonly AsyncLocal<string?> token = new();
    
    /// <summary>
    ///  Аутентификационный токен.
    /// </summary>
    public static string? Token => token.Value;
    
    
    /// <summary>
    /// Установить контекст.
    /// </summary>
    /// <param name="newToken">Токен.</param>
    /// <returns>Объект для восстановления контекста.</returns>
    public static IDisposable Set(string? newToken)
    {
        var revert = new ContextRollBack<string?>(token);
        token.Value = newToken;
        return revert;
    }
}