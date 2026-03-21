using Core.Domain.Enum.BookEnum;
using Core.Domain.Models.Inventory;
using Core.Domain.Repository;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class InventoryReadRepository : IInventoryReadRepository
{
    private readonly LibraryDbContext _dbContext;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="dbContext"><see cref="LibraryDbContext"/>.</param>
    public InventoryReadRepository(LibraryDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<CopyCountres>> GetCopyCountersAsync(IEnumerable<Guid> bookIds, CancellationToken ct = default)
    {
      var ids = bookIds as Guid[] ?? bookIds.ToArray();
      if (!ids.Any())
      {
          return Array.Empty<CopyCountres>();
      }

      var counters = await _dbContext.BookCopies
          .AsQueryable()
          .AsNoTracking()
          .Where(x => ids.Contains(x.BookId))
          .GroupBy(x => x.BookId)
          .Select(x => new CopyCountres
          {
              BookId = x.Key,
              Total = x.Count(),
              Available = x.Count(bookCopy => bookCopy.BookStatus == BookStatus.Available),
              Borrowed = x.Count(bookCopy => bookCopy.BookStatus == BookStatus.Borrowed),
              Booked = x.Count(bookCopy => bookCopy.BookStatus == BookStatus.Booked),
              Lost = x.Count(bookCopy => bookCopy.BookStatus == BookStatus.Lost)
          })
          .ToListAsync(ct);
      return counters;
    }

    /// <inheritdoc />
    public async Task<int> GetCopiesCountAsync(string title, Guid publisherId, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            return 0;
        }

        var bookId = await _dbContext.Books
            .AsQueryable()
            .AsNoTracking()
            .Where(t => t.Title == title && t.PublisherId == publisherId)
            .Select(x => x.Id)
            .SingleOrDefaultAsync(ct);
        if (bookId == Guid.Empty)
        {
            return 0;
        }

        return await _dbContext.BookCopies
            .AsNoTracking()
            .AsQueryable()
            .Where(x => x.BookId == bookId)
            .CountAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<(string Title, Guid PublisherId), bool>> BooksExistAsync(IEnumerable<(string Title, Guid PublisherId)> keys, CancellationToken ct = default)
    {
        var array = keys as (string Title, Guid PublisherId) [] ?? keys.ToArray();
        if (!array.Any())
        {
            return new Dictionary<(string Title, Guid PublisherId), bool>();
        }

        var existing = await _dbContext.Books
            .AsNoTracking()
            .AsQueryable()
            .Where(b => array.Select(k => k.Title).Contains(b.Title))
            .Select(b => new { b.Title, b.PublisherId })
            .ToListAsync(ct);
        
        var unique = existing
            .Select(x => (x.Title, x.PublisherId))
            .ToHashSet();
        
        var result = new Dictionary<(string Title, Guid PublisherId), bool>();
        foreach (var key in array)
        {
            result[key] = unique.Contains((key.Title, key.PublisherId));
        }
        return result;
    }

    /// <inheritdoc />
    public async Task<int> GetAvailableCopiesAsync(Guid bookId, CancellationToken ct = default)
        => await _dbContext.BookCopies
            .AsNoTracking()
            .AsQueryable()
            .Where(x => x.BookId == bookId && x.BookStatus == BookStatus.Available)
            .CountAsync(ct);
}