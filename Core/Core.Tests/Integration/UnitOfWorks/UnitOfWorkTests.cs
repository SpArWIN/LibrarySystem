using Common.Contracts.Constaints.Providers;
using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Db.Extensions;
using Common.Db.Factory;
using Common.Policies.Services;
using Core.Domain.Enum.BookEnum;
using Core.Domain.Models;
using Core.Infrastructure;
using Core.Infrastructure.Context;
using Core.Infrastructure.Extensions.UnitOfWorks;
using Core.Tests.Integration.Fixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core.Tests.Integration.UnitOfWorks;

/// <summary>
/// Интеграционные тесты на <see cref="IUnitOfWork"/>.
/// </summary>
[Collection(Provider.Postgres)]
public class UnitOfWorkTests(PostgresFixture fixture)
{
    private readonly IUnitOfWorkFactory<LibraryDbContext> _unitOfWorkFactory = fixture.ServiceProvider.GetRequiredService<IUnitOfWorkFactory<LibraryDbContext>>();
    
    private readonly IUnitOfWorkFactory<CentralDbContext> _centralFactory = fixture.ServiceProvider.GetRequiredService<IUnitOfWorkFactory<CentralDbContext>>();
    private readonly string _dbName = $"library-db-{Guid.NewGuid()}";
    private readonly IDatabaseProvisioner _databaseProvisioner = fixture.ServiceProvider.GetRequiredService<IDatabaseProvisioner>();
    private readonly ITenantDatabaseMigrator _tenantDatabaseMigrator = fixture.ServiceProvider.GetRequiredService<ITenantDatabaseMigrator>();
    private readonly IOptions<TenantProvisioningOptions> _options = fixture.ServiceProvider.GetRequiredService<IOptions<TenantProvisioningOptions>>();
    private readonly IDbResilience _dbResilience = fixture.ServiceProvider.GetRequiredService<IDbResilience>();
    
    private async Task<DataBaseSettings> CreateDataBaseAndMigrateTentantAsync()
    {
        await _databaseProvisioner.CreateDatabaseAsync(_dbName);
        var tenTantConnectionString = _options.Value.TenantConnectionStringTemplate
            .Replace("{db}", _dbName, StringComparison.Ordinal);
        var settings = new DataBaseSettings()
        {
            Provider = _options.Value.Provider,
            ConnectionString = tenTantConnectionString,
            MigrationsAssembly = _options.Value.MigrationsAssembly,
        };
        await _tenantDatabaseMigrator.MigrateAsync(settings);
        return settings;

    }
    
    /// <summary>
    /// Получить активные, не возвращёныые выдачи по списку пользователей.
    /// </summary>
    [Fact]
    public async Task GetActiveByUsersAsync_CreateActivityUsers_ReturnActivityLoan()
    {
        // arrange
        var settings = await CreateDataBaseAndMigrateTentantAsync();
        var userId = Guid.NewGuid();
      
        var bookId = Guid.NewGuid();
        await _centralFactory.CreateWithRetryAsync(action: async centralDb =>
        {
            centralDb.AddPreCommit(async (uow, ct) =>
            {
                var userRepository = uow.GetUserRepository();
                var existing = await userRepository.GetUsersByIdsAsync(new[] { userId }, ct);
                if (existing.Any())
                    throw new InvalidOperationException("User already exists");
            });

            centralDb.AddPostCommit((_, _) =>
            {
                Console.WriteLine($"User {userId} created at {DateTime.UtcNow}");
                return Task.CompletedTask;
            });
            var userRepository = centralDb.GetUserRepository();
            var user = new User()
            {
                Id = userId,
                Username = "admin",
                Password = "hashed_password",
                Name = "Test",
                LastName = "User",
                RegistrationDate = DateTime.UtcNow
            };
            await userRepository.AddUsersAsync(new[] { user });
            return user.Id;

            
        }, pipeline: _dbResilience.Write);

        await _unitOfWorkFactory.CreateWithRetryAsync(action:async libraryUow =>
        {
            var bookRepository = libraryUow.GetBookRepository();
            var publisherRepository = libraryUow.GetPublisherRepository();
            var publisher = new Publisher()
            {
                Id = Guid.NewGuid(),
                PublisherName = "Name",
                Address = "Address",
            };
            var publisherId = await  publisherRepository.AddPublishersAsync([publisher]);
            var book = new Book()
            {
                Id = bookId,
                Description = "Loan book",
                TotalCopies = 5,
                BookKey = "Key" + Guid.NewGuid(),
                PublisherId = publisherId.FirstOrDefault(),
                Title = "MainBook",
            };
           return await bookRepository.AddRangeAsync([book]);
           
            
        }, _dbResilience.Write, settings:settings);
        
        await _unitOfWorkFactory.CreateWithRetryAsync(async libraryUow =>
        {
            libraryUow.AddPreCommit(async (uow, ct) =>
            {
                var loanRepo = uow.GetLoanRepository();
                var activeLoans = await loanRepo.GetActiveByUsersAsync(new[] { userId }, ct);
                
                if (activeLoans.Any())
                    throw new InvalidOperationException("User already has active loans");
            });
            
            libraryUow.AddPostCommit( (_, _) =>
            {
                Console.WriteLine($"Loan created for user {userId} at {DateTime.UtcNow}");
                return Task.CompletedTask;
            });
          
            var loanRepo = libraryUow.GetLoanRepository();
            var bookCopyRepo = libraryUow.GetBookCopyRepository();
            var bookCopyId = await bookCopyRepo.GetCopiesIdsByBookIdAsync([bookId]);
            var loan = new Loan
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                BookCopyId = bookCopyId.FirstOrDefault(),
                LoanDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(14),
                IsReturned = false,
                BookStatus = BookStatus.Borrowed
            };
            var loanId = await loanRepo.AddRangeAsync(new[] { loan });
            
            return loanId;
           
        }, _dbResilience.Write, settings: settings);
        
        await using var queryUow = await _unitOfWorkFactory.CreateAsync(
            beginTransaction: false,
            ct: CancellationToken.None,
            dbSettings: settings);
        
        // assert
        var loanRepo = queryUow.GetLoanRepository();
        var activeLoans = await loanRepo.GetActiveByUsersAsync(new[] { userId }, CancellationToken.None);
        activeLoans.Should().NotBeEmpty();
        activeLoans.Should().HaveCount(1);
        activeLoans.First().UserId.Should().Be(userId);
        activeLoans.First().IsReturned.Should().BeFalse();
        
    }
 
}