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

    /// <summary>Опционально: выбранная библиотека (tenant).</summary>
    public Guid? LibraryId { get; init; }
}