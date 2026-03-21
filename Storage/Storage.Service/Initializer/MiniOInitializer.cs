using Storage.Application.Services;

namespace Storage.Service.Initializer;

/// <summary>
/// Инициализация бакетов.
/// </summary>
public sealed class MiniOInitializer : IHostedService
{
    private readonly IMinioBucketService _minioBucketService;

    /// <summary>
    /// Конструктор.
    /// </summary>
    /// <param name="minioBucketService"><see cref="IMinioBucketService"/>.</param>
    public MiniOInitializer(IMinioBucketService minioBucketService)
    {
        _minioBucketService = minioBucketService;
    }
    
    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _minioBucketService.StartAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
        => Task.CompletedTask;
}