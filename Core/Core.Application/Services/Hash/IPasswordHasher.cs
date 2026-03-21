namespace Core.Application.Services.Hash;

/// <summary>
/// Сервис хеширования.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Захешировать пароль.</summary>
    string Hash(string password);
    
    /// <summary>Проверить пароль на соответствие хэшу.</summary>
    bool Verify(string password, string passwordHash);
}