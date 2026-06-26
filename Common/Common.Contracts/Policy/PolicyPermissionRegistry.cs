using Common.Contracts.Permission;

namespace Common.Contracts.Policy;

/// <summary>
/// Соответствие имён политик ASP.NET Core и permission (scope) в JWT.
/// </summary>
public static class PolicyPermissionRegistry
{
    /// <summary>
    /// Все зарегистрированные пары политика → permission.
    /// </summary>
    public static IReadOnlyList<PolicyPermissionMapping> All { get; } =
    [
        new(PolicyNames.ViewDb, Permissions.ViewDb),
        new(PolicyNames.CreateDb, Permissions.CreateDb),
        new(PolicyNames.IssueBook, Permissions.IssueBook),
        new(PolicyNames.CreateUser, Permissions.RegisterUser),
    ];

    /// <summary>
    /// Найти permission по имени политики.
    /// </summary>
    public static string? GetPermission(string policyName) =>
        All.FirstOrDefault(m => m.Policy == policyName)?.Permission;
}

/// <summary>
/// Связь политики API и claim scope.
/// </summary>
/// <param name="Policy">Имя политики (<see cref="PolicyNames"/>).</param>
/// <param name="Permission">Значение claim scope (<see cref="Permissions"/>).</param>
public sealed record PolicyPermissionMapping(string Policy, string Permission);
