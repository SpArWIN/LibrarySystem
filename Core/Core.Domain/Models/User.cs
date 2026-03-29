namespace Core.Domain.Models;

/// <summary>
/// Сущность пользователя.
/// </summary>
public sealed class User
{
    /// <summary>
    /// Идентификатор пользователя.
    /// </summary>
    public Guid Id { get; init; }
    
    /// <summary>
    /// Логин пользователя.
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Фамилия пользователя.
    /// </summary>
    public required string LastName { get; init; }

    /// <summary>
    /// Имя пользователя.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Отчество пользователя.
    /// </summary>
    public string? SurName { get; init; }

    /// <summary>
    /// Дата рождения пользователя.
    /// </summary>
    public DateTime? DateOfBirth { get; init; }
    
    /// <summary>
    /// Пароль пользователя.
    /// </summary>
    public required string Password { get; init; }

    /// <summary>
    /// Адрес пользователя.
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// Номер телефона пользователя.
    /// </summary>
    public string? Phone { get; init; }

    /// <summary>
    /// Дата регистрации пользователя.
    /// </summary>
    public DateTime RegistrationDate { get; init; }

    /// <summary>
    /// Роли пользователя.
    /// </summary>
    public List<UserRole> UserRoles { get; init; } = [];
}