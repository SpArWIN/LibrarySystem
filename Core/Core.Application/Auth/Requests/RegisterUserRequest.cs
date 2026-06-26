using Common.Db.Abstractions;
using Core.Application.Auth.Context;
using Core.Domain.Models;

namespace Core.Application.Auth.Requests;

/// <summary>
/// MediatR-запрос регистрации пользователя в central DB.
/// </summary>
public class RegisterUserRequest : IUserOperationContext, IRequiresPasswordHashing
{
    /// <summary>Уникальный логин (username).</summary>
    public required string Username { get; init; }

    /// <inheritdoc />
    public required string Password { get; set; }

    /// <summary>Фамилия.</summary>
    public required string LastName { get; init; }

    /// <summary>Имя.</summary>
    public required string Name { get; init; }

    /// <summary>Отчество.</summary>
    public string? SurName { get; init; }

    /// <summary>Дата рождения.</summary>
    public required DateTime DateOfBirth { get; init; }

    /// <summary>Номер телефона.</summary>
    public string? Phone { get; init; }

    /// <summary>Почтовый адрес.</summary>
    public string? Address { get; init; }

    /// <inheritdoc />
    public Guid? LibraryId { get; init; }

    /// <summary>
    /// Идентификаторы ролей для назначения.
    /// Если не указаны — назначаются роли по умолчанию (Reader).
    /// </summary>
    public IReadOnlyCollection<Guid>? RoleIds { get; init; }

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
