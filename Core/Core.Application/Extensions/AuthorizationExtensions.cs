using Common.Contracts.Claims;
using Common.Contracts.Policy;
using Common.Extensions;
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
        services.AddAuthorization(options =>
        {
            PolicyPermissionRegistry.All.ForEach(pm =>
            {
                options.AddPolicy(pm.Policy, p =>
                    p.RequireClaim(ClaimNames.Scope, pm.Permission));
            });

            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });
        return services;
    }
}