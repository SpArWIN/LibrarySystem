using Common.Extensions;
using Core.Domain.Models.Instanse;
using Core.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

public sealed class RefreshSessionRepository : IRefreshSessionRepository
{
    private readonly CentralDbContext _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="context">.</param>
    public RefreshSessionRepository(CentralDbContext context)
    {
        _context = context;
    }
    
    /// <inheritdoc />
    public async Task AddAsync(RefreshSession session, CancellationToken ct = default)
    {
        await _context.RefreshSessions.AddAsync(session, ct);
    }

    /// <inheritdoc />
    public async Task<RefreshSession?> FindByHashAsync(string tokenHash, CancellationToken ct = default)
    {
        if (tokenHash.IsNullOrEmpty())
        {
            throw new ArgumentNullException(nameof(tokenHash), "Token hash cannot be null or empty.");
        }
        return await _context.RefreshSessions
            .AsQueryable()
            .AsNoTracking()
            .FirstOrDefaultAsync(x=> x.TokenHash == tokenHash, ct);
    }

    /// <inheritdoc />
    public async Task RevokeAsync(Guid sessionId, DateTimeOffset revokedAtUtc, Guid? replacedBySessionId,
        CancellationToken ct = default)
    {
        await _context.RefreshSessions
            .Where(x => x.Id == sessionId)
            .ExecuteUpdateAsync(settings => settings
                    .SetProperty(x => x.RevokedAtUtc, revokedAtUtc)
                    .SetProperty(x => x.ReplacedBySessionId, replacedBySessionId)
                , ct);
    }
}