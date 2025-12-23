namespace Common.Contracts.Storage.Files;

/// <summary>
/// Подписанный Url.
/// </summary>
public sealed record PresignedUrlResponse
{
    /// <summary>URL.</summary>
    public required string Url { get; init; }
    
    /// <summary>TTL ссылки, сек.</summary>
    public int ExpiresInSeconds { get; init; }
}