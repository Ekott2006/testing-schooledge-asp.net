// using System.Collections.Concurrent;
// using System.Text.Json;
// using Api.Helpers;
//
// namespace Api.BackgroundTask;
//
// public abstract class BaseFileProcessor<T>(
//     ILogger<BaseFileProcessor<T>> logger,
//     IServiceProvider serviceProvider,
//     string cacheFileName)
//     : BackgroundService
//     where T : class, new()
// {
//     public readonly ConcurrentQueue<string> _fileQueue = new();
//
//     private static readonly string
//         CacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DefaultCacheDirectory);
//
//     private readonly string _cacheFilePath = Path.Combine(CacheDir, cacheFileName);
//     public readonly SemaphoreSlim _semaphore = new(0);
//     private const string DefaultCacheDirectory = "cache";
//
//     public override Task StartAsync(CancellationToken cancellationToken)
//     {
//         Directory.CreateDirectory(CacheDir);
//         LoadQueueFromCache();
//         return base.StartAsync(cancellationToken);
//     }
//
//     public void QueueFile(string filePath)
//     {
//         if (string.IsNullOrWhiteSpace(filePath))
//         {
//             logger.LogError("File path cannot be empty. {}", nameof(filePath));
//             return;
//         }
//
//         _fileQueue.Enqueue(filePath);
//         _semaphore.Release();
//         logger.LogInformation("File {FilePath} enqueued for processing", filePath);
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         logger.LogInformation("Background task service is starting.");
//
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 await _semaphore.WaitAsync(stoppingToken);
//                 if (!_fileQueue.TryDequeue(out string? filePath)) continue;
//                 
//                 using IServiceScope scope = serviceProvider.CreateScope();
//                 logger.LogInformation("Started processing file: {FilePath}", filePath);
//                 await ProcessFileAsync(filePath, scope, stoppingToken);
//                 logger.LogInformation("Successfully processed file: {FilePath}", filePath);
//
//                 // Archive the file
//                 await ArchiveFileAsync(filePath);
//             }
//             catch (TaskCanceledException)
//             {
//                 // Ignore, as this is expected when the service is stopping.
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(ex, "An error occurred while processing a task.");
//             }
//         }
//     }
//
//     protected virtual async Task ProcessFileAsync(string filePath, IServiceScope scope,
//         CancellationToken cancellationToken)
//     {
//         if (string.IsNullOrWhiteSpace(filePath))
//             throw new ArgumentException("File path cannot be empty or null.");
//
//         if (!File.Exists(filePath))
//             throw new FileNotFoundException($"File not found: {filePath}");
//
//         string extension = Path.GetExtension(filePath).ToLower();
//         List<T> items = await ReadFileContentsAsync(filePath, extension, cancellationToken);
//
//         if (items.Count > 0)
//         {
//             await ProcessItemsAsync(items, scope, cancellationToken);
//             logger.LogInformation("Processed {Count} items from {FilePath}", items.Count, filePath);
//         }
//         else
//         {
//             logger.LogWarning("No items found in file {FilePath}", filePath);
//         }
//     }
//
//     protected virtual async Task<List<T>> ReadFileContentsAsync(string filePath, string extension,
//         CancellationToken cancellationToken)
//     {
//         List<T> items = [];
//
//         switch (extension)
//         {
//             case ".csv":
//                 await foreach (T item in DocumentConverter.ReadFromCsv<T>(filePath, cancellationToken))
//                 {
//                     items.Add(item);
//                 }
//
//                 break;
//
//             case ".xls" or ".xlsx":
//                 items = await DocumentConverter.ReadExcelAsync<T>(filePath, cancellationToken);
//                 break;
//
//             default:
//                 throw new NotSupportedException(
//                     $"File extension {extension} is not supported. Only .csv, .xls, and .xlsx files are supported.");
//         }
//
//         return items;
//     }
//
//     protected abstract Task ProcessItemsAsync(List<T> items, IServiceScope scope, CancellationToken cancellationToken);
//
//     private static async Task ArchiveFileAsync(string filePath)
//     {
//         string archivePath = Path.Combine(
//             Path.GetDirectoryName(filePath) ?? string.Empty,
//             "processed",
//             $"{Path.GetFileNameWithoutExtension(filePath)}_{DateTime.Now:yyyyMMddHHmmss}{Path.GetExtension(filePath)}"
//         );
//
//         Directory.CreateDirectory(Path.GetDirectoryName(archivePath) ?? string.Empty);
//         await Task.Run(() => File.Move(filePath, archivePath));
//     }
//
//
//     private async Task SaveQueueToCache()
//     {
//         try
//         {
//             await _semaphore.WaitAsync();
//             List<string> remainingFiles = _fileQueue.ToList();
//             if (remainingFiles.Count != 0)
//             {
//                 await File.WriteAllTextAsync(
//                     _cacheFilePath,
//                     JsonSerializer.Serialize(remainingFiles)
//                 );
//                 logger.LogInformation("Saved {Count} items to cache file", remainingFiles.Count);
//             }
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error saving queue to cache file");
//         }
//         finally
//         {
//             _semaphore.Release();
//         }
//     }
//
//     private void LoadQueueFromCache()
//     {
//         try
//         {
//             if (!File.Exists(_cacheFilePath)) return;
//             List<string>? cachedFiles = JsonSerializer.Deserialize<List<string>>(
//                 File.ReadAllText(_cacheFilePath)
//             );
//
//             if (cachedFiles != null)
//             {
//                 foreach (string file in cachedFiles)
//                 {
//                     _fileQueue.Enqueue(file);
//                 }
//
//                 logger.LogInformation("Loaded {Count} items from cache file", cachedFiles.Count);
//             }
//
//             // Clear the cache file after loading
//             File.Delete(_cacheFilePath);
//         }
//         catch (Exception ex)
//         {
//             logger.LogError(ex, "Error loading queue from cache file");
//         }
//     }
//
//     public override async Task StopAsync(CancellationToken cancellationToken)
//     {
//         await SaveQueueToCache();
//         await base.StopAsync(cancellationToken);
//     }
// }
//
// // Implementation for Students
//
// // Implementation for Questions
//
// using System.Collections.Concurrent;
//
// namespace Api.BackgroundTask;
//
// public class BackgroundTaskService(ILogger<BackgroundTaskService> logger) : BackgroundService
// {
//     private readonly ConcurrentQueue<string> _taskQueue = new();
//     private readonly SemaphoreSlim _signal = new(0);
//     private readonly ILogger<BackgroundTaskService> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//
//     public void QueueTask(string taskId)
//     {
//         if (string.IsNullOrEmpty(taskId))
//         {
//             throw new ArgumentException("Task ID cannot be null or empty.", nameof(taskId));
//         }
//
//         _taskQueue.Enqueue(taskId);
//         _signal.Release();
//         _logger.LogInformation("Task {TaskId} has been queued.", taskId);
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         _logger.LogInformation("Background task service is starting.");
//
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 await _signal.WaitAsync(stoppingToken);
//
//                 if (!_taskQueue.TryDequeue(out string? taskId)) continue;
//                 _logger.LogInformation("Processing task {TaskId}.", taskId);
//                 await ProcessTaskAsync(taskId, stoppingToken);
//             }
//             catch (TaskCanceledException)
//             {
//                 // Ignore, as this is expected when the service is stopping.
//             }
//             catch (Exception ex)
//             {
//                 _logger.LogError(ex, "An error occurred while processing a task.");
//             }
//         }
//
//         _logger.LogInformation("Background task service is stopping.");
//     }
//
//     private async Task ProcessTaskAsync(string taskId, CancellationToken cancellationToken)
//     {
//         try
//         {
//             for (int i = 1; i <= 10; i++)
//             {
//                 cancellationToken.ThrowIfCancellationRequested();
//
//                 _logger.LogInformation("Task {TaskId} progress: {Progress}%.", taskId, i * 10);
//
//                 await Task.Delay(1000, cancellationToken);
//             }
//
//             _logger.LogInformation("Task {TaskId} completed.", taskId);
//         }
//         catch (Exception ex)
//         {
//             _logger.LogError(ex, "An error occurred while processing task {TaskId}.", taskId);
//         }
//     }
// }
//
//
// using Domain.Dto.Question;
// using Domain.Repository;
//
// namespace Api.BackgroundTask;
//
// public class QuestionFileProcessor(
//     ILogger<QuestionFileProcessor> logger,
//     IServiceProvider serviceProvider)
//     : BaseFileProcessor<QuestionRequest>(logger, serviceProvider, "question-import-cache.json")
// {
//     protected override async Task ProcessItemsAsync(List<QuestionRequest> items, IServiceScope scope,
//         CancellationToken cancellationToken)
//     {
//         QuestionRepository repository = scope.ServiceProvider.GetRequiredService<QuestionRepository>();
//         await repository.BulkUpsert(items, cancellationToken);
//     }
//
// }
//
//
// using Domain.Dto.Student;
// using Domain.Model;
// using Domain.Model.Helpers;
// using Domain.Repository;
//
// namespace Api.BackgroundTask;
//
// public class StudentFileProcessor(
//     ILogger<StudentFileProcessor> logger,
//     IServiceProvider serviceProvider)
//     : BaseFileProcessor<StudentRequest>(logger, serviceProvider, "student-import-cache.json")
// {
//     public override Task StartAsync(CancellationToken cancellationToken)
//     {
//         return base.StartAsync(cancellationToken);
//     }
//
//     public override Task StopAsync(CancellationToken cancellationToken)
//     {
//         return base.StopAsync(cancellationToken);
//     }
//
//     protected override async Task ExecuteAsync(CancellationToken stoppingToken)
//     {
//         logger.LogInformation("Background task service is starting.");
//
//         while (!stoppingToken.IsCancellationRequested)
//         {
//             try
//             {
//                 await _semaphore.WaitAsync(stoppingToken);
//
//                 if (!_fileQueue.TryDequeue(out string? taskId)) continue;
//                 logger.LogInformation("Processing task {TaskId}.", taskId);
//                 // await ProcessTaskAsync(taskId, stoppingToken);
//             }
//             catch (TaskCanceledException)
//             {
//                 // Ignore, as this is expected when the service is stopping.
//             }
//             catch (Exception ex)
//             {
//                 logger.LogError(ex, "An error occurred while processing a task.");
//             }
//         }
//
//         logger.LogInformation("Background task service is stopping.");
//     }
//
//     protected override async Task ProcessItemsAsync(List<StudentRequest> items, IServiceScope scope,
//         CancellationToken cancellationToken)
//     {
//         AuthManager authManager = scope.ServiceProvider.GetRequiredService<AuthManager>();
//         StudentRepository repository = scope.ServiceProvider.GetRequiredService<StudentRepository>();
//
//         List<(string, StudentRequest)> successfulRegistrations = [];
//
//         foreach (StudentRequest request in items.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
//         {
//             (User user, Dictionary<string, string[]>? errors) =
//                 await authManager.Register(request, [UserRole.Student]);
//
//             if (errors != null)
//             {
//                 logger.LogWarning("Failed to register student {Email}: {Errors}",
//                     request.Email,
//                     string.Join(", ", errors.SelectMany(e => e.Value)));
//                 continue;
//             }
//
//             successfulRegistrations.Add((user.Id, request));
//         }
//
//         if (successfulRegistrations.Count <= 0) return;
//
//         await repository.CreateBulk(successfulRegistrations, cancellationToken);
//     }
// }