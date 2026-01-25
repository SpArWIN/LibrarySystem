using Core.Domain.Models;
using Core.Domain.Models.Pagination;
using Core.Domain.Repository;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class PublisherRepository : IPublisherRepository
{
    private readonly LibraryDbContext _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="context"><see cref="LibraryDbContext"/>.</param>
    public PublisherRepository(LibraryDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Publisher> Items, int TotalCount)> GetAllPublishersAsync(Pagination pagination, CancellationToken cancellationToken = default)
    {
        var pageNumber = pagination.PageNumber <= 0 ? 1 : pagination.PageNumber;
        var pageSize = pagination.PageSize <= 0 ? 1 : pagination.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        var query =
            _context.Publishers.AsQueryable()
            .AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);
        var pageIds = await query
            .Skip(skip)
            .Take(pageSize)
            .OrderBy(x=>x.PublisherName)
            .Select(x=>x.Id)
            .ToListAsync(cancellationToken);

        if (!pageIds.Any())
        {
            return (Enumerable.Empty<Publisher>(), 0);
        }

        var items = await query
            .Where(x => pageIds.Contains(x.Id))
            .Select(publisher => new Publisher
            {
                Id = publisher.Id,
                Address = publisher.Address,
                DateOfFound = publisher.DateOfFound,
                PublisherName = publisher.PublisherName,
            })
            .ToListAsync(cancellationToken);
        var ordered = items
            .OrderBy(x => pageIds.IndexOf(x.Id))
            .ToList();
        
        return (ordered, totalCount);

    }

    /// <inheritdoc />
    public async Task<IEnumerable<Publisher>> GetPublishersByIdsAsync(IEnumerable<Guid> publisherIds, CancellationToken cancellationToken = default)
    {
       var ids = publisherIds as Guid[] ?? publisherIds.ToArray();
       if (!ids.Any())
       {
           return Enumerable.Empty<Publisher>();
       }
       
       return await _context.Publishers
           .AsQueryable()
           .AsNoTracking()
           .Where(x => ids.Contains(x.Id))
           .AsSplitQuery()
           .ToListAsync(cancellationToken);
       
    }

    /// <inheritdoc />
    public async Task<Publisher> GetPublishersByParams(string parameters, CancellationToken cancellationToken = default)
    {
        return await _context.Publishers
            .AsQueryable()
            .AsNoTracking()
            .Where(x=>
                x.PublisherName.Contains(parameters) 
                      || x.Address.Contains(parameters))
            .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IEnumerable<Guid>> AddPublishersAsync(IEnumerable<Publisher> publishers , 
        CancellationToken cancellationToken = default)
    {
      var publisherList = publishers as Publisher[] ?? publishers.ToArray();
      if (!publisherList.Any())
      {
          return new List<Guid>();
      }
      await _context.Publishers.AddRangeAsync(publisherList, cancellationToken);
      return publisherList.Select(x=>x.Id);
    }

    /// <inheritdoc />
    public ValueTask UpdatePublishersAsync(IEnumerable<Publisher> publishers, CancellationToken cancellationToken = default)
    {
      var publisherList = publishers as Publisher[] ?? publishers.ToArray();
      if (!publisherList.Any())
      {
          return ValueTask.CompletedTask;
      }
      _context.Publishers.UpdateRange(publisherList);
      
      return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DeletePublishersAsync(IEnumerable<Guid> publisherIds, CancellationToken cancellationToken = default)
    {
        var ids  = publisherIds as Guid[] ?? publisherIds.ToArray();
        if (!ids.Any())
        {
            return;
        }

        await _context.Publishers
            .AsQueryable()
            .Where(x => ids.Contains(x.Id))
            .ExecuteDeleteAsync(cancellationToken);
        
    }

    /// <inheritdoc />
    public async Task<List<Guid>> GetMissingPublisherIdsAsync(IEnumerable<Guid> publisherIds, CancellationToken cancellationToken = default)
    {
      return await _context.Publishers
          .AsQueryable()
          .AsNoTracking()
          .Where(x=> publisherIds.Contains(x.Id))
          .Select(x => x.Id)
          .ToListAsync(cancellationToken);
    }
}