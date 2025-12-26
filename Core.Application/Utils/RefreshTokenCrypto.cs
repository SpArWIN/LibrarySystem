using System.Security.Cryptography;
using System.Text;

namespace Core.Application.Utils;

/// <summary>
/// Хеширование.
/// </summary>
public static class RefreshTokenCrypto
{
    /// <summary>Сгенерировать refresh token.</summary>
    public static string GenerateToken()
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        return Base64UrlEncode(bytes);
    }
    
    public static string ComputeHash(string token, string pepper)
    {
        var input = $"{pepper}:{token}";
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
    
    private static string Base64UrlEncode(ReadOnlySpan<byte> bytes)
        => Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}