using Common.Extensions;
using Core.Domain.Enum.BookEnum;
using Core.Domain.Filter;
using Core.Domain.Models;
using Core.Domain.Models.Loans;
using Core.Domain.Models.Pagination;
using Core.Domain.Repository;
using Core.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Core.Infrastructure.Repository;

/// <inheritdoc />
public sealed class LoanRepository : ILoanRepository
{
    private readonly LibraryDbContext _context;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="context"><see cref="LibraryDbContext"/>.</param>
    public LoanRepository(LibraryDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc />
    public async Task<Loan?> GetByIdAsync(Guid loanId, CancellationToken cancellationToken = default)
    {
        return await _context.Loans
            .AsNoTracking()
            .Include(x=>x.BookCopy)
            .FirstOrDefaultAsync(x=>x.Id == loanId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Loan> Items, int TotalCount)> GetPageAsync(LoanCriteria criteria, Pagination page, CancellationToken cancellationToken = default)
    {
        var query = ApplyCriteria(_context.Loans.AsNoTracking(), criteria);
        var pageNumber = page.PageNumber <= 0 ? 1 : page.PageNumber;
        var pageSize = page.PageSize <= 0 ? 1 : page.PageSize;
        var skip = (pageNumber - 1) * pageSize;
        var totalCount = await query.CountAsync(cancellationToken);
        
        var loanItems = await query
            .OrderByDescending(x=>x.LoanDate)
            .ThenByDescending(x=>x.Id)
            .Skip(skip)
            .Take(pageSize)
            .Include(x=>x.BookCopy)
            .ToListAsync(cancellationToken);
        return (loanItems, totalCount);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Loan>> GetActiveByUsersAsync(IEnumerable<Guid> userIds, CancellationToken cancellationToken = default)
    {
       var ids = userIds as Guid[] ?? userIds.ToArray();
       if (!ids.Any())
       {
           return new List<Loan>();
       }
       return await _context.Loans
           .AsNoTracking()
           .AsQueryable()
           .Where(x=> x.UserId != null && ids.Contains(x.UserId.Value))
           .Where(x=> !x.IsReturned)
           .OrderByDescending(x=>x.LoanDate)
           .ToListAsync(cancellationToken);
       
    }

    /// <inheritdoc />
    public async Task<Loan?> GetActiveWithUserByBookIdAsync(Guid bookId, CancellationToken cancellationToken = default)
    {
       return await _context.Loans
           .Include(x=>x.BookCopy)
           .Where(x=>!x.IsReturned)
           .Where(x=>x.BookCopy != null && x.BookCopy.BookId == bookId)
           .OrderByDescending(x=>x.LoanDate)
           .FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<Guid>> AddRangeAsync(IEnumerable<Loan> loans, CancellationToken cancellationToken = default)
    {
      var loanList = loans as Loan[] ?? loans.ToArray();
      if (!loanList.Any())
      {
          return new List<Guid>();
      }
      await _context.Loans.AddRangeAsync(loanList, cancellationToken);
      return loanList.Select(x=>x.Id).ToList();
    }

    /// <inheritdoc />
    public async Task UpdateRangeAsync(IEnumerable<LoanPatch> updates, CancellationToken cancellationToken = default)
    {
        foreach (var patch in updates)
        {
            await _context.Loans
                .Where(x => x.Id == patch.Id)
                .ExecuteUpdateAsync(s => s
                        .SetProperty(x => x.BookStatus,  x => patch.BookStatus ?? x.BookStatus)
                        .SetProperty(x => x.IsReturned,  x => patch.IsReturned ?? x.IsReturned)
                        .SetProperty(x => x.ExpiryDate,  x => patch.ExpiryDate ?? x.ExpiryDate)
                        .SetProperty(x => x.OverdueDays, x => patch.OverdueDays ?? x.OverdueDays)
                        .SetProperty(x => x.UserId,      x => patch.UserId ?? x.UserId)
                        .SetProperty(x => x.BookCopyId,  x => patch.BookId ?? x.BookCopyId),
                    cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task DeleteRangeAsync(IEnumerable<Guid> loanIds, CancellationToken cancellationToken = default)
    {
      var ids = loanIds as Guid[] ?? loanIds.ToArray();
      if (!ids.Any())
      {
          return;
      }

      await _context.Loans
          .AsQueryable()
          .Where(x => ids.Contains(x.Id))
          .ExecuteDeleteAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task UpdateStatusRangeAsync(IEnumerable<Guid> loanIds, BookStatus newStatus, DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        var ids = loanIds as Guid[] ?? loanIds.ToArray();
        if (!ids.Any())
        {
            return;
        }
        await _context.Loans
            .Where(x=> ids.Contains(x.Id))
            .ExecuteUpdateAsync(setting => setting.SetProperty(x=>x. BookStatus, newStatus), cancellationToken);
    }

    /// <inheritdoc />
    public async Task ReturnRangeAsync(IEnumerable<Guid> loanIds, DateTime returnUtc, CancellationToken cancellationToken = default)
    {
       var ids = loanIds as Guid[] ?? loanIds.ToArray();
       if (!ids.Any())
       {
           return;
       }
       await _context.Loans
           .Where(x=> ids.Contains(x.Id))
           .ExecuteUpdateAsync(setting => setting
               .SetProperty(x=>x.IsReturned, true)
               .SetProperty(x=>x.BookStatus, BookStatus.Available)
               .SetProperty(x => x.OverdueDays,
                   x => x.ExpiryDate < returnUtc
                       ? (returnUtc.Date - x.ExpiryDate.Date).Days
                       : null),
               cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> MarkOverdueAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        var overdue = await _context.Loans
            .AsNoTracking()
            .Where(x=> !x.IsReturned)
            .Where(x=>x.BookStatus == BookStatus.Borrowed)
            .Where(x=>x.ExpiryDate < nowUtc)
            .Select(x=> new{x.Id, x.ExpiryDate})
            .ToListAsync(cancellationToken);
        
        if (!overdue.Any())
        {
            return 0;
        }
        
        var ids = overdue.Select(x=>x.Id).ToArray();
        await _context.Loans
            .Where(x=> ids.Contains(x.Id))
            .ExecuteUpdateAsync(settings =>
                settings.SetProperty(x=>x.BookStatus, BookStatus.Overdue), cancellationToken);

        return overdue.Count;
    }

    /// <inheritdoc />
    public async Task  CancelExpiredBookingsAsync(DateTime nowUtc, CancellationToken cancellationToken = default)
    {
        await _context.Loans
            .Where(x=> !x.IsReturned)
            .Where(x=>x.BookStatus == BookStatus.Booked)
            .Where(x=> x.ExpiryDate < nowUtc)
            .ExecuteDeleteAsync(cancellationToken);
    }

    private static IQueryable<Loan> ApplyCriteria(IQueryable<Loan> query, LoanCriteria criteria)
    {
        if (criteria.UserId is not null)
        {
            query = query.Where(x=>x.UserId == criteria.UserId);
        }

        if (criteria.BookId is not null)
        {
            query = query.Where(x=>x.BookCopy != null && x.BookCopy.BookId == criteria.BookId);
        }

        if (criteria.BookKey.IsNotNullOrEmpty())
        {
            query = query.Where(x=>x.BookCopy != null && x.BookCopy.BookKey == criteria.BookKey);
        }

        if (criteria.IsActiveOnly)
        {
            query = query.Where(x => !x.IsReturned);
        }

        if (criteria.OverdueOnly is true)
        {
            query = query.Where(x => x.BookStatus == BookStatus.Overdue);
        }

        if (criteria.Statuses is { Count: > 0 })
        {
            query = query.Where(x=>criteria.Statuses.Contains(x.BookStatus));
        }

        return query;
    }
}