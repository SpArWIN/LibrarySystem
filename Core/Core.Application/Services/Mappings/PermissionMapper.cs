using Common.Contracts.Permission;
using Core.Domain.Enum.Roles;

namespace Core.Application.Services.Mappings;

/// <inheritdoc />
public sealed class PermissionMapper : IPermissionMapper
{
    /// <inheritdoc />
    public IReadOnlyCollection<string> MapScopes(IReadOnlyCollection<Roles> roles)
    {
        if (roles is null)
        {
            return [];
        }
        var scopes = new HashSet<string>(StringComparer.Ordinal);
        foreach (var role in roles)
        {
            foreach (var scope in GetScopes(role))
                scopes.Add(scope);
        }
        
        return scopes.ToArray();
    }

    private static IReadOnlyCollection<string> GetScopes(Roles role)
        => role switch
        {
            Roles.Reader =>
            [
                Permissions.GetBook
            ],
            Roles.Librarian =>
            [
                Permissions.GetBook,
                Permissions.IssueBook,
                Permissions.ViewDb
            ],
            Roles.Administrator =>
            [
                Permissions.GetBook,
                Permissions.IssueBook,
                Permissions.ViewDb,
                Permissions.CreateDb
            ],
            _ => []
        };
}