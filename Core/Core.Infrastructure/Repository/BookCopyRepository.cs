using Core.Domain.Enum.BookEnum;
using Core.Domain.Models;
using Core.Domain.Repository;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class BookCopyRepository : IBookCopyRepository
{
    private readonly LibraryDbContext _dbContext;
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="dbContext"><see cref="LibraryDbContext"/>.</param>
    public BookCopyRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    
    /// <inheritdoc />
    public async Task<IReadOnlyList<BookCopy>> GetCopiesByBookIdAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default)
    {
        return await _dbContext.BookCopies
            .AsQueryable()
            .AsNoTracking()
            .Where(x => bookIds.Contains(x.BookId))
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Guid>> GetCopiesIdsByBookIdAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default)
    {
        return await _dbContext.BookCopies
            .AsQueryable()
            .Where(x => bookIds.Contains(x.BookId))
            .Select(x => x.Id)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<BookCopy>> GetAvailableCopiesByBookIdAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default)
    {
        return await _dbContext.BookCopies
            .AsQueryable()
            .AsNoTracking()
            .Where(x => bookIds.Contains(x.BookId) && x.BookStatus == BookStatus.Available)
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task UpdateCopiesAsync(IEnumerable<Guid> copyIds, BookStatus newStatus, CancellationToken ct = default)
    {
        await _dbContext.BookCopies
            .AsQueryable()
            .Where(x => copyIds.Contains(x.Id))
            .ExecuteUpdateAsync(s =>
                s.SetProperty(x => x.BookStatus, newStatus), ct);
    }
    
}