using System.Security.Claims;
using Common.Contracts.Auth;

namespace Core.Application.Services.Permissions;

/// <summary>
/// Проверка прав и ролей текущего пользователя по claims JWT.
/// </summary>
public interface IPermissionEvaluator
{
    /// <summary>
    /// Есть ли у пользователя permission (claim <c>scope</c>).
    /// </summary>
    bool HasPermission(ClaimsPrincipal user, string permission);

    /// <summary>
    /// Есть ли у пользователя роль (claim <c>role</c>).
    /// </summary>
    bool HasRole(ClaimsPrincipal user, string role);

    /// <summary>
    /// Выполняется ли политика ASP.NET (через связанный permission из <see cref="Common.Contracts.Policy.PolicyPermissionRegistry"/>).
    /// </summary>
    bool HasPolicy(ClaimsPrincipal user, string policyName);

    /// <summary>
    /// Собрать роли, permissions и доступные политики из JWT.
    /// </summary>
    CurrentUserPermissionsDto GetCurrentAccess(ClaimsPrincipal user);
}
