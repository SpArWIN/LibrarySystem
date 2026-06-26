namespace Common.Contracts.Auth;

/// <summary>
/// Модель для логина.
/// </summary>
public sealed record LoginRequestDto
{
    /// <summary>Логин пользователя.</summary>
    public string Username { get; init; } = null!;

    /// <summary>Пароль.</summary>
    public  string Password { get; init; } = null!;

    /// <summary>Опционально: refresh-токен клиента для повторной выдачи той же сессии.</summary>
    public string? RefreshToken { get; init; }

    /// <summary>Опционально: выбранная библиотека (tenant).</summary>
    public Guid? LibraryId { get; init; }
}