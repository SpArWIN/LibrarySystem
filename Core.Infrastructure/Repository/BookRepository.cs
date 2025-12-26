using Core.Domain.Models;
using Core.Domain.Models.Pagination;
using Core.Domain.Repository;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class BookRepository : IBookRepository
{
    public async Task<(IEnumerable<Book> Items, int TotalCount)> GetAllBooksAsync(Pagination pagination, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Book>> GetBooksByIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public IAsyncEnumerable<Book> StreamAllAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteBooks(IEnumerable<Guid> booksIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task AddRangeAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Book>> UpdateRangeAsync(IEnumerable<Book> books, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task RemoveBooksIdsAsync(IEnumerable<Guid> bookIds, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}