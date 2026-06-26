using Common.Db.Abstractions;
using Core.Application.Auth.Context;
using Core.Domain.Models;

namespace Core.Application.Auth.Requests;

/// <summary>
/// MediatR-запрос обновления пары access/refresh токенов.
/// </summary>
public sealed class RefreshTokensRequest : IRefreshTokenOperationContext
{
    /// <inheritdoc />
    public required string SubmittedRefreshToken { get; init; }

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
