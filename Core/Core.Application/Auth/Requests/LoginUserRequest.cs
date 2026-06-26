using Common.Db.Abstractions;
using Core.Application.Auth.Context;
using Core.Domain.Models;

namespace Core.Application.Auth.Requests;

/// <summary>
/// MediatR-запрос входа по логину и паролю.
/// </summary>
public class LoginUserRequest : IUserOperationContext
{
    /// <summary>Логин пользователя.</summary>
    public required string Username { get; init; }

    /// <summary>Пароль в открытом виде (проверяется против хэша в БД).</summary>
    public required string Password { get; init; }

    /// <summary>
    /// Опциональный refresh-токен клиента.
    /// Если сессия ещё активна — вернётся та же пара (или только access, если refresh не передан).
    /// </summary>
    public string? SubmittedRefreshToken { get; init; }

    /// <inheritdoc />
    public Guid? LibraryId { get; init; }

    /// <inheritdoc />
    public required IUnitOfWork UnitOfWork { get; init; }

    /// <inheritdoc />
    public User? User { get; set; }

    /// <inheritdoc />
    public string AccessToken { get; set; } = string.Empty;

    /// <inheritdoc />
    public DateTimeOffset AccessExpiresAtUtc { get; set; }

    /// <inheritdoc />
    public string RefreshToken { get; set; } = string.Empty;

    /// <inheritdoc />
    public DateTimeOffset RefreshExpiresAtUtc { get; set; }

    /// <inheritdoc />
    public string?[] Roles { get; set; } = [];

    /// <inheritdoc />
    public IReadOnlyCollection<string> Permissions { get; set; } = [];
}
