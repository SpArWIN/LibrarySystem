namespace Common.Contracts.Auth;

/// <summary>
/// Запрос на регистрацию пользователя.
/// </summary>
public sealed class RegisterRequestDto
{
    /// <summary>Логин.</summary>
    public required string Username { get; init; }
    
    /// <summary>Пароль.</summary>
    public required string Password { get; init; }
    
    /// <summary>Фамилия.</summary>
    public required string LastName { get; init; }
    
    /// <summary>Имя.</summary>
    public required string Name { get; init; }
    
    /// <summary>Отчество (опционально).</summary>
    public string? SurName { get; init; }
    
    /// <summary>Дата рождения.</summary>
    public required DateTime DateOfBirth { get; init; }
    
    /// <summary>Телефон.</summary>
    public string? Phone { get; init; }
    
    /// <summary>Адрес.</summary>
    public string? Address { get; init; }
    
    /// <summary>
    /// Опционально: библиотека для scope в JWT при регистрации.
    /// Обычно <c>null</c> — пользователь ещё не привязан к tenant.
    /// </summary>
    public Guid? LibraryId { get; init; }
    
    /// <summary>
    /// Идентификаторы ролей для назначения пользователю.
    /// Если не указаны — присваивается роль Reader по умолчанию.
    /// </summary>
    public IReadOnlyCollection<Guid>? RoleIds { get; init; }
}