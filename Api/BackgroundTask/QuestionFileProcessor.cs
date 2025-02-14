using Domain.Dto.Question;
using Domain.Repository;

namespace Api.BackgroundTask;

public class QuestionFileProcessor(
    IServiceProvider serviceProvider,
    ILogger<QuestionFileProcessor> logger) : BaseFileProcessor<QuestionRequest>(serviceProvider,
    logger, "question-import-cache.json")
{
    protected override async Task ProcessListAsync(IServiceScope scope, List<QuestionRequest> fileList, CancellationToken stoppingToken)
    {
        QuestionRepository repository = scope.ServiceProvider.GetRequiredService<QuestionRepository>();
        await repository.BulkUpsert(fileList, stoppingToken);
    }
}