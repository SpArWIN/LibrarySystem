namespace Core.Application.Auth.Context;

/// <summary>
/// Запрос с паролем в открытом виде (будет захеширован в пайплайне до сохранения в БД).
/// </summary>
public interface IRequiresPasswordHashing
{
    /// <summary>
    /// Пароль в открытом виде на входе; после <c>PasswordHashPipelineBehavior</c> — хэш.
    /// </summary>
    string Password { get; set; }
}
