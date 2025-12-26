namespace Common.Contracts.Auth;

/// <summary>
/// Запрос на выход.
/// </summary>
public class LogoutRequestDto
{
    /// <summary>token, который нужно отозвать.</summary>
    public required string RefreshToken { get; init; }
}