namespace Common.Contracts.User;

/// <summary>
/// Информация о пользователе.
/// </summary>
public sealed record UserInfoDto
{
    /// <summary>Идентификатор пользователя.</summary>
    public required Guid Id { get; init; }

    /// <summary>Логин пользователя.</summary>
    public required string Username { get; init; }

    /// <summary>Фамилия пользователя.</summary>
    public required string LastName { get; init; }

    /// <summary>Имя пользователя.</summary>
    public required string Name { get; init; }

    /// <summary>Отчество пользователя.</summary>
    public string? SurName { get; init; }

    /// <summary>Дата рождения пользователя.</summary>
    public DateTime? DateOfBirth { get; init; }

    /// <summary>Адрес пользователя.</summary>
    public string? Address { get; init; }

    /// <summary>Номер телефона пользователя.</summary>
    public string? Phone { get; init; }

    /// <summary>Дата регистрации.</summary>
    public required DateTime RegistrationDate { get; init; }

    /// <summary>Роли пользователя (строковые имена).</summary>
    public required IReadOnlyCollection<string> Roles { get; init; }

    /// <summary>
    /// Permissions (scopes из JWT) — для отображения возможностей на фронте после login/register.
    /// </summary>
    public required IReadOnlyCollection<string> Permissions { get; init; }
}