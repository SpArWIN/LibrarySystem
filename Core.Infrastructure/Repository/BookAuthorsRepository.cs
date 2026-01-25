using Core.Domain.Models;
using Core.Domain.Repository;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class BookAuthorsRepository : IBookAuthorsRepository
{
    private readonly LibraryDbContext _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="context"><see cref="LibraryDbContext"/>.</param>
    public BookAuthorsRepository(LibraryDbContext context)
    {
        _context = context;
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<Author?>> GetAuthorsByBooksIds(IEnumerable<Guid> booksIds, CancellationToken cancellationToken = default)
    {
        var ids = booksIds as Guid[] ?? booksIds.ToArray();
        if (!ids.Any())
        {
            return Enumerable.Empty<Author?>();
        }
        var authors = await _context.AuthorBooks
            .AsNoTracking()
            .Where(ba => ids.Contains(ba.BookId))
            .Select(ba => ba.Author)
            .Distinct()
            .ToListAsync(cancellationToken);
        return authors;
    }

    /// <inheritdoc />
    public async Task<List<BookAuthor>> GetAssociationsAsync(IEnumerable<Guid> authorIds, IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default)
    {
        var authorIdArray = authorIds as Guid[] ?? authorIds.ToArray();
        var bookIdArray = bookIds as Guid[] ?? bookIds.ToArray();
        if (!authorIdArray.Any() || !bookIdArray.Any())
        {
            return new List<BookAuthor>();
        }

        var associations = await _context.AuthorBooks
            .AsNoTracking()
            .Where(authorBook => authorIdArray.Contains(authorBook.AuthorId) && bookIdArray.Contains(authorBook.BookId))
            .Include(x => x.Book)
            .Include(x => x.Author)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
        
        return associations;

    }

    /// <inheritdoc />
    public async Task<IEnumerable<Book?>> GetBooksByAuthorsIdsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default)
    {
        var ids = authorIds as Guid[] ?? authorIds.ToArray();
        if (!ids.Any())
        {
            return Enumerable.Empty<Book?>();
        }
        var bookIds = await _context.AuthorBooks
            .AsNoTracking()
            .Where(ba => ids.Contains(ba.AuthorId))
            .Select(ba => ba.BookId)
            .Distinct()
            .ToListAsync(cancellationToken);
        if (!bookIds.Any())
        {
            return Enumerable.Empty<Book?>();
        }
        var books = await _context.Books
            .AsNoTracking()
            .Where(b => bookIds.Contains(b.Id))
            .Include(b => b.Publisher)
            .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
            .Include(b => b.BookAuthors)
            .ThenInclude(ba => ba.Author)
            .AsSplitQuery()
            .ToListAsync(cancellationToken);
        return books;
        
    }

    /// <inheritdoc />
    public async Task AddAuthorsToBooksAsync(IEnumerable<(Guid AuthorId, Guid BookId)> associations)
    {
        var associationList = associations as (Guid AuthorId, Guid BookId)[] ?? associations.ToArray();
        if (!associationList.Any())
        {
            return;
        }
        
        var newAssociations = await GetNewAssociationsOptimizedAsync(associationList);
       
        if (newAssociations.Any())
        {
            await _context.AuthorBooks.AddRangeAsync(newAssociations);
        }

    }

    /// <inheritdoc />
    public ValueTask RemoveRangeAsync(IEnumerable<BookAuthor> associations)
    {
        var associationList = associations as BookAuthor[] ?? associations.ToArray();
        if (!associationList.Any())
        {
            return ValueTask.CompletedTask;
        }

        _context.AuthorBooks.RemoveRange(associationList);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<bool> IsAuthorAssociatedWithBookAsync(Guid authorId, Guid bookId)
    {
        return await _context.AuthorBooks
            .AsNoTracking()
            .AnyAsync(ba => ba.AuthorId == authorId && ba.BookId == bookId);
    }

    private async Task<List<BookAuthor>> GetNewAssociationsOptimizedAsync(
        IEnumerable<(Guid AuthorId, Guid BookId)> associations)
    {
        var associationArray = associations.ToArray();
        var associationsData = associationArray
            .Select(a => new { a.AuthorId, a.BookId })
            .Distinct()
            .ToList();
        if (!associationsData.Any())
        {
            return new List<BookAuthor>();
        }
        var newAssociations = await _context.AuthorBooks
            .Where(ba => associationsData
                .Select(a => new { a.AuthorId, a.BookId })
                .Contains(new { ba.AuthorId, ba.BookId }))
            .Select(ba => new { ba.AuthorId, ba.BookId })
            .ToListAsync()
            .ContinueWith(task =>
            {
                var existingPairs = task.Result.ToHashSet();
                return associationsData
                    .Where(a => !existingPairs.Contains(a))
                    .Select(a => new BookAuthor
                    {
                        Id = Guid.NewGuid(),
                        AuthorId = a.AuthorId,
                        BookId = a.BookId
                    })
                    .ToList();
            });
        return newAssociations;
    }
}