using Core.Infrastructure.Context;
using DotNet.Testcontainers.Containers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Tests.Integration.Fixture;

/// <summary>
/// Абстрактный класс для построения фикстур разных баз данных.
/// </summary>
public abstract class DataBaseFixture : IAsyncLifetime
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private IServiceProvider _serviceProvider = null!;
    private  bool _initialized = false;
    private bool _isDisposed = false;

    /// <summary>Контейнер бд.</summary>
    protected IContainer? DataBaseContainer { get; }

    /// <summary>Admin connection string (к серверу/системной БД).</summary>
    public string AdminConnectionString { get; private set; } = null!;
    
    
    /// <summary>
    /// Строка подключения к центральной базе данныз. БД.
    /// </summary>
    public string CentralConnectionString { get; protected set; } = null!;
    
    /// <summary>
    /// Поставщик.
    /// </summary>
    public IServiceProvider ServiceProvider
        => _initialized ? _serviceProvider : 
            throw new InvalidOperationException("Fixture must be initialized.");
    
    /// <summary>
    /// Конфигурация.
    /// </summary>
    public IConfiguration Configuration { get; private set; } = null!;

    /// <summary>
    /// Имя тестовой базы данных.
    /// </summary>
    public string DatabaseName { get; }
    
    /// <summary>
    /// Конструктор.
    /// </summary>
    protected DataBaseFixture()
    {
        DatabaseName = GenerateTestDatabaseName();
        DataBaseContainer = CreateDataBaseContainer();
    }

    /// <summary>
    /// Построить основную строку подключения к базе.
    /// </summary>
    /// <param name="baseConnectionString">Админская строка подключения.</param>
    /// <returns>Строка подключения.</returns>
    protected abstract string BuildAdminConnectionString(string baseConnectionString);
    
    /// <summary>
    /// Построить конкретную строку подключения к базе.
    /// </summary>
    /// <param name="baseConnectionString">Админская строка подключения.</param>
    /// <param name="databaseName">Имя базы данных.</param>
    /// <returns>Строка подключения.</returns>
    protected abstract string BuildDatabaseConnectionString(string baseConnectionString, string databaseName);
    
    /// <summary>
    /// Создать основную базу данных.
    /// </summary>
    /// <param name="baseConnectionString">Админская строка подключения.</param>
    /// <param name="databaseName">Название базы данных.</param>
    protected abstract Task CreateDatabaseAsync(string baseConnectionString, string databaseName);
    
    /// <summary>
    /// Создать контейнер базы данных.
    /// </summary>
    /// <returns><see cref="IContainer"/>.</returns>
    protected abstract IContainer CreateDataBaseContainer();
    
    /// <summary>
    /// Получить строку подключения из контейнера.
    /// </summary>
    /// <returns>Строка подключения.</returns>
    protected abstract string GetConnectionStringFromContainer();
    
    /// <summary>
    /// Настраивает DbContextOptionsBuilder.
    /// </summary>
    protected abstract void ConfigureDbContext(DbContextOptionsBuilder optionsBuilder);
    
    /// <summary>
    /// Настроить DI.
    /// </summary>
    /// <param name="services"><see cref="IServiceCollection"/>.</param>
    /// <returns><see cref="IServiceCollection"/>.</returns>
    protected abstract IServiceCollection ConfigureServices(IServiceCollection services);
    
    /// <summary>
    /// Возвращает имя провайдера базы данных (для конфигурации).
    /// </summary>
    protected abstract string GetDatabaseProviderName();
    
    /// <summary>
    /// Очищает тестовые данные после выполнения тестов.
    /// </summary>
    protected abstract Task CleanupTestDataAsync();

    /// <summary>
    /// Применить необходимые миграции.
    /// </summary>
    /// <param name="serviceProvider"><see cref="IServiceProvider"/>.</param>
    /// <returns>.</returns>
    protected abstract Task ApplyCentralMigrationsAsync(IServiceProvider serviceProvider);
    
    protected string GenerateTestDatabaseName()
    {
        var testClass = GetType().Name.ToLower().Replace("fixture", "");
        return $"test_{testClass}_{Guid.NewGuid():N}";
    }

    private IConfiguration BuildTestConfiguration()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.test.json", optional: true)
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Database:Provider"] = GetDatabaseProviderName(),
                ["ConnectionStrings:Admin"] = AdminConnectionString,
                ["ConnectionStrings:Central"] = CentralConnectionString,
            })
            .AddEnvironmentVariables()
            .Build();
    }
    
    
    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        if (_initialized)
        {
            return;
        }
        await _semaphore.WaitAsync();
        try
        {
            await DataBaseContainer?.StartAsync()!;
            var baseConnectionString = GetConnectionStringFromContainer();
            AdminConnectionString = BuildAdminConnectionString(baseConnectionString);
            CentralConnectionString = BuildDatabaseConnectionString(baseConnectionString, DatabaseName);
            Configuration = BuildTestConfiguration();
            
            var services = new ServiceCollection();
            services.AddSingleton(Configuration);
            ConfigureServices(services);
            
            var optionsBuilder = new DbContextOptionsBuilder<LibraryDbContext>();
            ConfigureDbContext(optionsBuilder);
            
            _serviceProvider = services.BuildServiceProvider();
            await ApplyCentralMigrationsAsync(_serviceProvider);
            _initialized = true;
        }
        catch (Exception)
        {
            await CleanupTestDataAsync();
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            await CleanupTestDataAsync();
        }
        finally
        {
            
            if (DataBaseContainer is not null)
            {
                await DataBaseContainer.DisposeAsync();
            }
            _isDisposed = true;
            _semaphore.Release();
            _semaphore.Dispose();
        }
    }
}