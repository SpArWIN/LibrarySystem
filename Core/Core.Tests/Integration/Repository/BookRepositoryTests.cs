using Common.Contracts.Constaints.Providers;
using Common.Contracts.Settings;
using Common.Db.Abstractions;
using Common.Db.Extensions;
using Common.Db.Factory;
using Common.Policies.Services;
using Core.Domain.Models;
using Core.Domain.Models.Pagination;
using Core.Infrastructure.Context;
using Core.Infrastructure.Extensions.UnitOfWorks;
using Core.Tests.Integration.Fixture;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Core.Tests.Integration.Repository;
[Collection(Provider.Postgres)]
public sealed class BookRepositoryTests(PostgresFixture fixture)
{
   
    private readonly IUnitOfWorkFactory<LibraryDbContext> _unitOfWorkFactory = fixture.ServiceProvider.GetRequiredService<IUnitOfWorkFactory<LibraryDbContext>>();
    private readonly ITenantDatabaseMigrator _tenantDatabaseMigrator = fixture.ServiceProvider.GetRequiredService<ITenantDatabaseMigrator>();
    private readonly IOptions<TenantProvisioningOptions> _options = fixture.ServiceProvider.GetRequiredService<IOptions<TenantProvisioningOptions>>();
    private readonly IDatabaseProvisioner _databaseProvisioner = fixture.ServiceProvider.GetRequiredService<IDatabaseProvisioner>();
    private readonly IDbResilience _dbResilience = fixture.ServiceProvider.GetRequiredService<IDbResilience>();
    private readonly string _dbName = $"library-db-{Guid.NewGuid()}";

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
    /// Пустые данные.
    /// </summary>
    [Fact]
    public async Task GetAllBooksAsync_EmptyDataBase_EmptyCollection()
    {
        // arrange
        var settings = await CreateDataBaseAndMigrateTentantAsync();
        await using var  uow = await _unitOfWorkFactory.CreateAsync(dbSettings:settings);
        
         //act
         var repository = uow.GetBookRepository();
         var (items, total) = 
             await repository.GetAllBooksAsync(
             new Pagination()
             {
                 PageSize = 1,
                 PageNumber = 1
             }
         );
         
         //assert
         items.Should().BeEmpty();
         total.Should().Be(0);
    }

    /// <summary>
    /// Получить список всех книг с пагинацией.
    /// </summary>
    [Fact]
    public async Task GetAllBooksAsync_NotEmptyDataBase_Collection()
    {
        // arrange
        var settings = await CreateDataBaseAndMigrateTentantAsync();

        var pagination = new Pagination()
        {
            PageSize = 10,
            PageNumber = 1
        };
        
        // act
      var bookRepository = await _unitOfWorkFactory.CreateWithRetryAsync(async uow =>
      {
          var bookRepository = uow.GetBookRepository();
          var publisherRepository = uow.GetPublisherRepository();
          var publisherId = Guid.NewGuid();
          await publisherRepository.AddPublishersAsync(new Publisher[]
          {
              new Publisher()
              {
                  Id = publisherId,
                  PublisherName = "Test Publisher",
              }
          }, cancellationToken:CancellationToken.None);
          await bookRepository.AddRangeAsync(Enumerable.Range(1, 25).Select(i => new Book()
          {
              Id = Guid.NewGuid(),
              Title = $"Book {i:D5}",
              PublisherId = publisherId,
              BookKey = $"Book Key {i:D5}",
              TotalCopies = 2
          }),cancellationToken: CancellationToken.None);

          return bookRepository;

      }, pipeline:_dbResilience.Write,
          cancellationToken:CancellationToken.None, settings);
      
            // assert
        var result = await bookRepository.GetAllBooksAsync(pagination, CancellationToken.None);
        result.TotalCount.Should().Be(25);
        result.Items.Should().HaveCount(10);
    }

    /// <summary>
    /// Получить список книг по их идентификаторам.
    /// </summary>
    [Fact]
    public async Task GetBooksByIdsAsync_NotEmptyDataBase_Collection()
    {
        // arrange 
        var settings = await CreateDataBaseAndMigrateTentantAsync();
        var randomGuids = GenerateGuids(5);
        var booksIds = await _unitOfWorkFactory.CreateWithRetryAsync(async uow =>
        {
            var bookRepository = uow.GetBookRepository();
            var publisherRepository = uow.GetPublisherRepository();
            var publisherId = Guid.NewGuid();
            await publisherRepository.AddPublishersAsync(new Publisher[]
            {
                new Publisher()
                {
                    Id = publisherId,
                    PublisherName = "Test Publisher",
                }
            }, cancellationToken:CancellationToken.None);
            
          var booksIds = await bookRepository.AddRangeAsync(Enumerable.Range(1, 11).Select(i => new Book()
            {
                Id = Guid.NewGuid(),
                Title = $"Book {i:D5}",
                PublisherId = publisherId,
                BookKey = $"Book Key {i:D5}",
                TotalCopies = 2
            }),cancellationToken: CancellationToken.None);
            return booksIds;
            
        }, pipeline:_dbResilience.Write, cancellationToken:CancellationToken.None, settings);
        
        // act
        var conccatGuids = randomGuids.Concat(booksIds);
        var result = await _unitOfWorkFactory.ExecuteQueryAsync(async uow =>
        {
            var bookRepository = uow.GetBookRepository();
            var books = await bookRepository.GetBooksByIdsAsync(conccatGuids, CancellationToken.None);
            return books;
        }, pipeline:_dbResilience.Read, CancellationToken.None, settings);
    
        
        // assert
        var books = result as Book[] ?? result.ToArray();
        books.Should().NotBeEmpty();
        books.Select(x=>x.Id).Should().BeEquivalentTo(booksIds);
    }
    
    private List<Guid> GenerateGuids(int count)
    {
        return Enumerable.Range(0, count)
            .Select(_ => Guid.NewGuid())
            .ToList();
    }
    
}