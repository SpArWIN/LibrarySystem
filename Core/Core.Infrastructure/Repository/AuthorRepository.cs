using Core.Domain.Models;
using Core.Domain.Models.Pagination;
using Core.Domain.Repository;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class AuthorRepository : IAuthorRepository
{
    private readonly LibraryDbContext _context;
    
    /// <summary>
    /// КОнструктор.
    /// </summary>
    /// <param name="context"><see cref="LibraryDbContext"/>.</param>
    public AuthorRepository(LibraryDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<Author> items, int totalCount)> GetAuthorsAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
        var pageSize = pagination.PageSize <= 0 ? 1 : pagination.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        var query = _context.Authors.AsQueryable()
            .AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var pageIds = await query
            .AsNoTracking()
            .AsQueryable()
            .OrderBy(x=>x.Name)
            .Skip(skip)
            .Take(pageSize)
            .Select(x=>x.Id)
            .ToListAsync(cancellationToken);
        if (!pageIds.Any())
        {
            return (Enumerable.Empty<Author>(), 0);
        }
        var items = await query
            .AsQueryable()
            .Where(x=> pageIds.Contains(x.Id))
            .Select(x=> new Author
            {
                Id = x.Id,
                Country = x.Country,
                DateOfBirth = x.DateOfBirth,
                Name = x.Name,
                LastName = x.LastName,
                SurName = x.SurName
            })
            .OrderBy(x => pageIds.IndexOf(x.Id)) 
            .ToListAsync(cancellationToken);
      
        
        return (items, totalCount);

    }

    public async Task<IEnumerable<Author>> GetAuthorsByIdsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default)
    {
        return await _context.Authors
            .AsQueryable()
            .AsNoTracking()
            .Where(x=> authorIds.Contains(x.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Author> GetByParamsAsync(string parameters)
    {
        return await _context.Authors
            .AsQueryable()
            .AsNoTracking()
            .Where(x => x.Name.Contains(parameters) ||
                        x.SurName.Contains(parameters) ||
                        x.Country.Contains(parameters) || x.LastName.Contains(parameters))
            .AsSingleQuery()
            .FirstOrDefaultAsync();
    }
    
    /// <inheritdoc />
    public async IAsyncEnumerable<Author> StreamAllAuthorsAsync(int batchSize, CancellationToken cancellationToken = default)
    {
        if (batchSize <= 0)
        {
            yield break;
        }

        var baseQuery = _context.Authors.AsQueryable()
            .AsNoTracking()
            .OrderBy(x => x.Id);
        Guid? lastId = null;
        List<Author> authors;
        do
        {
            var query = lastId is null
                ? baseQuery
                : baseQuery.Where(x => x.Id.CompareTo(lastId.Value) > 0);
            
             authors = await 
                query
                    .Take(batchSize)
                    .ToListAsync(cancellationToken);
             foreach (var item in authors)
            {
                yield return item;
            }
            lastId = authors.Count > 0 ? authors.Last().Id : null;
        } while (authors.Count > 0 && cancellationToken.IsCancellationRequested);

    }

    /// <inheritdoc />
    public async Task DeleteAuthorsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default)
    {
        var ids = authorIds as Guid[] ?? authorIds.ToArray();
        if (!ids.Any())
        {
            return;
        }
        await _context.Authors
            .AsQueryable()
            .Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);
    }
    /// <inheritdoc />
    public async Task<List<Guid>> AddRangeAsync(IEnumerable<Author> authors, CancellationToken cancellationToken = default)
    {
       var authorsList = authors as Author[] ?? authors.ToArray();
       if (!authorsList.Any())
       {
           return new List<Guid>();
       }
       await _context.Authors
           .AddRangeAsync(authorsList, cancellationToken);
       return authorsList.Select(x=>x.Id).ToList();
    }
    
    /// <inheritdoc />
    public async ValueTask<IEnumerable<Author>> UpdateAuthorsAsync(IEnumerable<Author> authors, CancellationToken cancellationToken = default)
    {
       var authorsList = authors as Author[] ?? authors.ToArray();
       if (!authorsList.Any())
       {
           return await ValueTask.FromResult<IEnumerable<Author>>([]);
       }
       _context.Authors.UpdateRange(authorsList);
       return await ValueTask.FromResult(authorsList);
    }

    /// <inheritdoc />
    public async Task RemoveAuthorIdsAsync(IEnumerable<Guid> authorIds, CancellationToken cancellationToken = default)
    {
      await DeleteAuthorsAsync(authorIds, cancellationToken);
    }
}