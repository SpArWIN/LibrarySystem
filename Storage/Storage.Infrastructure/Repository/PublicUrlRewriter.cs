using Common.Contracts.Storage.Settings;
using Common.Extensions;
using Microsoft.Extensions.Options;
using Storage.Domain.Repository;

namespace Storage.Infrastructure.Repository;

/// <inheritdoc />
public sealed class PublicUrlRewriter : IPublicUrlRewriter
{
    private readonly IOptions<MinioOptions> _options;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="options"></param>
    public PublicUrlRewriter(IOptions<MinioOptions> options)
    {
        _options = options;
    }

    /// <inheritdoc />
    public string Rewrite(string url)
    {
        if (_options.Value.PublicEndpoint.IsNullOrEmpty())
        {
            return url;
        }
        var uri = new Uri(url);

        return new UriBuilder(uri)
        {
            Scheme = _options.Value.WithSsl ? "https" : "http",
            Host   = _options.Value.PublicEndpoint,
            Port   = _options.Value.PublicPort > 0 ? _options.Value.PublicPort : -1
        }.Uri.ToString();
    }
}