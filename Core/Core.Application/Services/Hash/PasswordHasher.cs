namespace Core.Application.Services.Hash;

/// <inheritdoc />
public sealed class PasswordHasher : IPasswordHasher
{
    /// <inheritdoc />
    public string Hash(string password)
    {
        ArgumentNullException.ThrowIfNull(password);
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <inheritdoc />
    public bool Verify(string password, string passwordHash)
    {
        ArgumentNullException.ThrowIfNull(password);
        ArgumentNullException.ThrowIfNull(passwordHash);
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}