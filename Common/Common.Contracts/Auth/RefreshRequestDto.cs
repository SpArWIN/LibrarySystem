namespace Common.Contracts.Auth;

/// <summary>
/// Запрос на обновление токена.
/// </summary>
public sealed class RefreshRequestDto
{
    /// <summary>Refresh token.</summary>
    public required string RefreshToken { get; init; }

    /// <summary>Опционально: библиотека, в контексте которой выдаём access.</summary>
    public Guid? LibraryId { get; init; }
}