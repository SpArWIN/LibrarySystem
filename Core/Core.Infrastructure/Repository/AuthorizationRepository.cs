using Core.Domain.Models;
using Core.Domain.Models.Instanse;
using Core.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class AuthorizationRepository(CentralDbContext context) : IAuthorizationRepository
{
    /// <inheritdoc />
    public async Task<User?> FindUserByUsernameAsync(string username, CancellationToken ct = default)
     => await context.Users
         .AsQueryable()
         .Include(x=>x.UserRoles)
         .ThenInclude(x=>x.Role)
         .FirstOrDefaultAsync(x=>x.Username == username,ct);

    /// <inheritdoc />
    public async Task<User?> FindUserByIdAsync(Guid userId, CancellationToken ct = default)
    => await context.Users
        .Include(x=>x.UserRoles)
        .ThenInclude(x=>x.Role)
        .FirstOrDefaultAsync(x=>x.Id == userId,ct);

    /// <inheritdoc />
    public async Task<RefreshSession?> FindRefreshSessionByHashAsync(string tokenHash, CancellationToken ct = default)
    =>  await context.RefreshSessions.FirstOrDefaultAsync(x=> x.TokenHash == tokenHash,ct);

    /// <inheritdoc />
    public async Task AddUserRolesAsync(IEnumerable<UserRole> userRoles, CancellationToken ct = default)
    {
        var roleList = userRoles as UserRole[] ?? userRoles.ToArray();
        if (!roleList.Any()) return;
        
        await context.UserRoles.AddRangeAsync(roleList, ct);
    }


    /*
    /// <inheritdoc />
    public async Task AddRefreshSessionAsync(RefreshSession session, CancellationToken ct = default)
     => await context.RefreshSessions.AddAsync(session,ct);

    /// <inheritdoc />
    public async Task RevokeRefreshSessionAsync(Guid sessionId, DateTimeOffset revokedAtUtc, Guid? replacedBySessionId,
        CancellationToken ct = default)
    {
        var session = await context.RefreshSessions.FirstOrDefaultAsync(x => x.Id == sessionId, ct);
        if (session is null)
        {
            return;
        }
        session.RevokedAtUtc = revokedAtUtc;
        session.ReplacedBySessionId = replacedBySessionId;
    }*/
}