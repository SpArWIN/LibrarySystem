using Common.Cached.Configurations;
using Common.Contracts.User;
using Microsoft.Extensions.Options;

namespace Common.Cached.PrefixStrategy.Strategy;

public class UserCachePrefixStrategy : CachePrefixStrategy<UserInfoDto>
{
    private readonly CachePrefixesOptions _options;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="options"><see cref="CachePrefixesOptions"/>.</param>
    public UserCachePrefixStrategy(IOptions<CachePrefixesOptions> options)
    {
        _options = options.Value;
    }

    /// <inheritdoc />
    public override string GetPrefix() => _options.User;
}