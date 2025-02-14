using System.Collections.Concurrent;
using Api.Helpers;

namespace Api.BackgroundTask;

public abstract class BaseFileProcessor<T> : BackgroundService where T : new()
{
    private readonly string _cacheFilePath;
    private readonly CacheService<string> _cacheService;
    private readonly ConcurrentQueue<string> _fileQueue = new();
    private readonly SemaphoreSlim _signal = new(0);
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BaseFileProcessor<T>> _logger;

    protected BaseFileProcessor(IServiceProvider serviceProvider, ILogger<BaseFileProcessor<T>> logger,
        string cacheFilePath)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cacheFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
        _cacheService = new CacheService<string>(logger, _cacheFilePath);
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        List<string> cacheAsync = await _cacheService.ReadCacheAsync();
        cacheAsync.ForEach(x => _fileQueue.Enqueue(x));
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _cacheService.WriteCacheAsync(_fileQueue.ToList());
        await base.StopAsync(cancellationToken);
    }

    public void QueueFile(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            _logger.LogError("File Path cannot be null or empty. {}", nameof(filePath));

        _fileQueue.Enqueue(filePath);
        _signal.Release();
        _logger.LogInformation("File Path {TaskId} has been queued.", filePath);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Background task service is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _signal.WaitAsync(stoppingToken);
                // await Task.Delay(30 * 1000, stoppingToken);
                if (!_fileQueue.TryDequeue(out string? filePath)) continue;

                _logger.LogInformation("Started processing file: {FilePath}", filePath);
                using IServiceScope scope = _serviceProvider.CreateScope();
                List<T> list = await ReadFileContentsAsync(filePath, stoppingToken);

                await ProcessListAsync(scope, list, stoppingToken);
                _logger.LogInformation("Successfully processed file: {FilePath}", filePath);
            }
            // Ignore, as this is expected when the service is stopping.
            catch (TaskCanceledException)
            {
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing a task.");
            }
        }

        _logger.LogInformation("Background task service is stopping.");
    }

    protected abstract Task ProcessListAsync(IServiceScope scope, List<T> fileList, CancellationToken stoppingToken);

    private static async Task<List<T>> ReadFileContentsAsync(string oldFilePath, CancellationToken cancellationToken)
    {
        string filePath = Path.Combine(FileHelper.FileLocation, oldFilePath);
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"File not found: {filePath}");
        string extension = Path.GetExtension(filePath).ToLower();
        List<T> items = [];

        switch (extension)
        {
            case ".csv":
                await foreach (T item in DocumentConverter.ReadFromCsv<T>(filePath, cancellationToken))
                {
                    items.Add(item);
                }

                break;

            case ".xls" or ".xlsx":
                items = await DocumentConverter.ReadExcelAsync<T>(filePath, cancellationToken);
                break;

            default:
                throw new NotSupportedException(
                    $"File extension {extension} is not supported. Only .csv, .xls, and .xlsx files are supported.");
        }

        return items;
    }
}