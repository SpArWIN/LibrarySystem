using System.Security.Claims;
using Common.Contracts.Claims;

namespace Core.Application.Builder;

/// <inheritdoc />
public sealed class ClaimBuilder : IClaimBuilder
{
    /// <inheritdoc />
    public IReadOnlyCollection<Claim> BuildAccessClaims(
        Guid userId,
        string username,
        IReadOnlyCollection<string> roleNames,
        IReadOnlyCollection<string> scopes,
        Guid? libraryId)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString("D")),
            new Claim(ClaimTypes.Name, username),

        };
        claims.AddRange(roleNames.Select(role => new Claim(ClaimTypes.Role, role)));
        
        claims.AddRange(scopes.Select(scope => new Claim(ClaimNames.Scope, scope)));

        if (libraryId.HasValue)
        {
            claims.Add(new Claim(ClaimNames.LibraryId, libraryId.Value.ToString("D")));
        }
        return claims;
    }
}