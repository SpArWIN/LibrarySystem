using System.Security.Claims;
using Common.Contracts.Auth;
using Common.Contracts.Claims;
using Common.Contracts.Policy;

namespace Core.Application.Services.Permissions;

/// <inheritdoc cref="IPermissionEvaluator"/>
public sealed class PermissionEvaluator : IPermissionEvaluator
{
    /// <inheritdoc />
    public bool HasPermission(ClaimsPrincipal user, string permission) =>
        user.HasClaim(ClaimNames.Scope, permission);

    /// <inheritdoc />
    public bool HasRole(ClaimsPrincipal user, string role) =>
        user.IsInRole(role);

    /// <inheritdoc />
    public bool HasPolicy(ClaimsPrincipal user, string policyName)
    {
        var permission = PolicyPermissionRegistry.GetPermission(policyName);
        return permission is not null && HasPermission(user, permission);
    }

    /// <inheritdoc />
    public CurrentUserPermissionsDto GetCurrentAccess(ClaimsPrincipal user)
    {
        var userId = Guid.Parse(
            user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("UserId claim is missing."));

        var username = user.FindFirstValue(ClaimTypes.Name)
                         ?? throw new InvalidOperationException("Username claim is missing.");

        var roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var permissions = user.FindAll(ClaimNames.Scope).Select(c => c.Value).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
        var permissionSet = permissions.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var policies = PolicyPermissionRegistry.All
            .Where(m => permissionSet.Contains(m.Permission))
            .Select(m => m.Policy)
            .ToArray();

        Guid? libraryId = Guid.TryParse(user.FindFirstValue(ClaimNames.LibraryId), out var id) ? id : null;

        return new CurrentUserPermissionsDto
        {
            UserId = userId,
            Username = username,
            Roles = roles,
            Permissions = permissions,
            Policies = policies,
            LibraryId = libraryId,
        };
    }
}
