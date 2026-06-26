using Permissionses = Common.Contracts.Permission.Permissions;
using Common.Extensions;
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
        
        roles.ForEach(role =>
        {
            GetScopes(role).ForEach(scope =>
            {
                scopes.Add(scope);
            });
        });
        
        return scopes.ToArray();
    }

    private static IReadOnlyCollection<string> GetScopes(Roles role)
        => role switch
        {
            Roles.Reader =>
            [
                Permissionses.GetBook
            ],
            Roles.Librarian =>
            [
                Permissionses.GetBook,
                Permissionses.IssueBook,
                Permissionses.ViewDb
            ],
            Roles.Administrator =>
            [
                Permissionses.GetBook,
                Permissionses.IssueBook,
                Permissionses.ViewDb,
                Permissionses.CreateDb,
                Permissionses.RegisterUser 
            ],
            _ => []
        };
}