using Common.Contracts.Claims;
using Common.Contracts.Permission;
using Common.Contracts.Policy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Application.Extensions;

/// <summary>
/// Расширения на назначения авторизации.
/// </summary>
public static class AuthorizationExtensions
{
    public static IServiceCollection AddPermissionPolicies(this IServiceCollection services)
    {
        var map = new (string Policy, string Permission)[]
        {
            (PolicyNames.ViewDb, Permissions.ViewDb),
            (PolicyNames.CreateDb, Permissions.CreateDb),
            (PolicyNames.IssueBook, Permissions.IssueBook)
        };

        services.AddAuthorization(options =>
        {
            foreach (var (policy, permission) in map)
            {
                options.AddPolicy(policy, p => p.RequireClaim(ClaimNames.Scope, permission));
            }

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        return services;
    }
}