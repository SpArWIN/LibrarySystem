using System.Runtime.CompilerServices;
using Core.Domain.Enum.BookEnum;
using Core.Domain.Models;
using Core.Domain.Models.Inventory;
using Core.Domain.Models.Pagination;
using Core.Domain.Repository;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class BookRepository : IBookRepository
{
    private readonly LibraryDbContext _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="context"><see cref="LibraryDbContext"/>.</param>
    public BookRepository(LibraryDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<BookListItem> Items, int TotalCount)> GetAllBooksAsync(Pagination pagination, 
        CancellationToken cancellationToken = default)
    {
        var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
        var pageSize = pagination.PageSize <= 0 ? 1 : pagination.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        var query = _context.Books
            .AsQueryable()
            .AsNoTracking()
            .Include(x => x.Publisher)
            .Include(b => b.BookGenres)
            .ThenInclude(x => x.Genre);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var pageIds = await query
            .AsNoTracking()
            .AsQueryable()
            .OrderBy(x=>x.Title)
            .Skip(skip)
            .Take(pageSize)
            .Select(x=>x.Id)
            .ToListAsync(cancellationToken);
        
        if (!pageIds.Any())
        {
            return ([], 0);
        }
        
        var items = await query
            .AsQueryable()
            .Where(x=> pageIds.Contains(x.Id))
            .Select(b => new BookListItem
            {
                BookId = b.Id,
                Title = b.Title,
                PublisherId = b.PublisherId,
                PublisherName = b.Publisher.PublisherName,
                TotalCopies = _context.BookCopies.Count(c=> c.BookId == b.Id),
                AvailableCopies = _context.BookCopies.Count(c=> c.BookId == b.Id 
                                                                &&
                                                                c.BookStatus == BookStatus.Available),
                Genres = b.BookGenres.Select(x=>x.Genre.NameGenre).ToList()
            })
            // ReSharper disable once EntityFramework.UnsupportedServerSideFunctionCall
            .OrderBy(x => pageIds.IndexOf(x.BookId))
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default)
    {
       var ids = bookIds as Guid[] ?? bookIds.ToArray();
       if (!ids.Any())
       {
           return [];
       }
       return await _context.Books
           .AsQueryable()
           .AsNoTracking()
           .Where(x=> ids.Contains(x.Id))
           .Include(x=>x.BookAuthors)
           .ThenInclude(x=> x.Author)
           .Include(x=>x.BookGenres)
           .ThenInclude(x=>x.Genre)
           .AsSplitQuery()
           .ToListAsync(cancellationToken);
       
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Book> StreamAllAsync(int batchSize,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (batchSize <= 0)
        {
            yield break;
        }

        var baseQuery = _context.Books
            .AsNoTracking()
            .AsQueryable()
            .OrderBy(x => x.Id);
        
        Guid? lastId = null;
        List<Book> batchBook;
        do
        {
            var query = lastId is null
                ? baseQuery
                : baseQuery.Where(x => x.Id.CompareTo(lastId.Value) > 0);
            
            batchBook = await 
                query.Take(batchSize)
                    .ToListAsync(cancellationToken);
            foreach (var book in batchBook)
            {
                yield return book;
            }
            
            lastId = batchBook.Count >0 ? batchBook.Last().Id : null;
        }
        while (batchBook.Count > 0 && cancellationToken.IsCancellationRequested);
    }

    /// <inheritdoc />
    public async Task DeleteBooks(IEnumerable<Guid> booksIds, CancellationToken cancellationToken = default)
    {
        var ids = booksIds as Guid[] ?? booksIds.ToArray();
        if (!ids.Any())
        {
            return;
        }

        await _context.Books
            .AsQueryable()
            .Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Guid>> AddRangeAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
       var bookList = books as Book[] ?? books.ToArray();
       if (!bookList.Any())
       {
           return new List<Guid>();
       }
       await _context.Books
           .AddRangeAsync(bookList, cancellationToken);
       return bookList.Select(x => x.Id).ToList();
    }

    /// <inheritdoc />
    public ValueTask<IEnumerable<Book>> UpdateRangeAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
        var list = books as Book[] ?? books.ToArray();
        if (!list.Any())
        {
            return ValueTask.FromResult<IEnumerable<Book>>([]) ;
        }
        _context.Books.UpdateRange(list);
        return ValueTask.FromResult<IEnumerable<Book>>(list);
    }

    /// <inheritdoc />
    public async Task RemoveBooksIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default)
    {
        await DeleteBooks(bookIds, cancellationToken);
    }
}