using Common.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Common.Http;

/// <summary>
/// Извлечение JWT из заголовка <c>Authorization</c>.
/// </summary>
public static class BearerTokenReader
{
    private const string BearerTokenHeaderName = "Authorization";
    private const string BearerTokenHeaderScheme = "Bearer";
    
    /// <summary>
    /// Прочитать access token из <see cref="HttpRequest.Headers"/> Authorization.
    /// </summary>
    /// <param name="headers">Заголовки запроса.</param>
    /// <returns>JWT без префикса Bearer или <c>null</c>.</returns>
    public static string? TryReadFromHeaders(IHeaderDictionary headers)
    {
        if (!headers.TryGetValue(BearerTokenHeaderName, out var values))
        {
            return null;
        }
        
        foreach (var value in values)
        {
            var token = TryParseAuthorizationValue(value);
            if (token.IsNotNullOrEmpty())
            {
                return token;
            }
        }

        return null;
    }

    /// <summary>
    /// Разобрать значение заголовка Authorization.
    /// </summary>
    private static string? TryParseAuthorizationValue(string? headerValue)
    {
        if (headerValue.IsNullOrWhiteSpace())
        {
            return null;
        }

        var trimmed = headerValue?.Trim();
        
        if (trimmed?.StartsWith(BearerTokenHeaderScheme, StringComparison.OrdinalIgnoreCase) is true)
        {
            return trimmed[BearerTokenHeaderScheme.Length..].Trim();
        }

        var parts = trimmed?.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts?.Length == 2 && parts[0].Equals(BearerTokenHeaderScheme, StringComparison.OrdinalIgnoreCase))
        {
            return parts[1];
        }
        
        return parts?.Length == 1 ? parts[0] : null;
    }
}
