using Core.Domain.Models;
using Core.Domain.Models.Pagination;
using Core.Domain.Repository;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class BookGenreRepository : IBookGenreRepository
{
    private readonly LibraryDbContext _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="context"><see cref="LibraryDbContext"/>.</param>
    public BookGenreRepository(LibraryDbContext context)
    {
        _context = context;
    }
    
    /// <inheritdoc />
    public async Task<(IEnumerable<BookGenre?> Items, int TotalCount)> GetAllBookGenresAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
        var pageSize = pagination.PageSize <= 0 ? 1 : pagination.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        var baseQuery = _context.BookGenres
            .AsQueryable()
            .AsNoTracking()
            .Include(x => x.Book)
            .Include(x => x.Genre);
        var totalCount = await baseQuery.CountAsync(cancellationToken);
        
        var pageIds = await baseQuery
            .OrderBy(x => x.BookId)
            .ThenBy(x => x.GenreId)
            .Skip(skip)
            .Take(pageSize)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);
        if (!pageIds.Any())
        {
            return (Enumerable.Empty<BookGenre?>(), 0);
        }
        
        var items = await baseQuery
            .Where(x=> pageIds.Contains(x.Id))
            .Select(x=> new BookGenre
            {
                Id = x.Id,
                BookId = x.BookId,
                Book = x.Book,
                GenreId = x.GenreId,
                Genre = x.Genre
            })
            .OrderBy(x=> pageIds.IndexOf(x.Id))
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BookGenre?>> GetGenresByBooksIdsAsync(IEnumerable<Guid> booksIds, CancellationToken cancellationToken = default)
    {
        var ids = booksIds as Guid[] ?? booksIds.ToArray();
        if (!ids.Any())
        {
            return Enumerable.Empty<BookGenre?>();
        }
        return await _context.BookGenres
            .AsNoTracking()
            .Where(x => ids.Contains(x.BookId))
            .Include(x => x.Genre)
            .Include(x => x.Book)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BookGenre?>> GetBooksByGenreIdAsync(IEnumerable<Guid> genreIds, CancellationToken cancellationToken = default)
    {
        var ids = genreIds as Guid[] ?? genreIds.ToArray();
        if (!ids.Any())
        {
            return Enumerable.Empty<BookGenre?>();
        }
        return await _context.BookGenres
            .AsNoTracking()
            .Where(x => ids.Contains(x.GenreId))
            .Include(x => x.Book)
            .ThenInclude(b => b.Publisher)
            .Include(x => x.Book)
            .ThenInclude(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .Include(x => x.Genre)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
            
    }

    /// <inheritdoc />
    public async Task<IEnumerable<BookGenre>> CreateBookGenresAsync(IEnumerable<BookGenre> bookGenres)
    {
        var bookGenreList = bookGenres as BookGenre[] ?? bookGenres.ToArray();
        if (!bookGenreList.Any())
        {
            return Enumerable.Empty<BookGenre>();
        }
        await _context.BookGenres.AddRangeAsync(bookGenreList);
        return bookGenreList;
    }

    /// <inheritdoc />
    public async Task<bool> IsBookGenreExistsAsync(Guid bookId, Guid genreId)
    {
        return await _context.BookGenres
            .AsNoTracking()
            .AnyAsync(x => x.BookId == bookId && x.GenreId == genreId);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Guid>> DeleteBookGenresAsync(IEnumerable<BookGenre> bookGenres)
    {
        var bookGenreList = bookGenres as BookGenre[] ?? bookGenres.ToArray();
        if (!bookGenreList.Any())
        {
            return Enumerable.Empty<Guid>();
        }
        var idsToDelete = bookGenreList.Select(x => x.Id).ToList();
        await _context.BookGenres
            .Where(x => idsToDelete.Contains(x.Id))
            .ExecuteDeleteAsync();
        return idsToDelete;
    }
}