using Domain.Dto.Student;
using Domain.Model;
using Domain.Model.Helpers;
using Domain.Repository;

namespace Api.BackgroundTask;

public class StudentFileProcessor(
    IServiceProvider serviceProvider,
    ILogger<StudentFileProcessor> logger) : BaseFileProcessor<StudentRequest>(serviceProvider,
    logger, "student-import-cache.json")
{
    protected override async Task ProcessListAsync(IServiceScope scope, List<StudentRequest> fileList,
        CancellationToken cancellationToken)
    {
        AuthManager authManager = scope.ServiceProvider.GetRequiredService<AuthManager>();
        StudentRepository repository = scope.ServiceProvider.GetRequiredService<StudentRepository>();

        List<(string, StudentRequest)> successfulRegistrations = [];

        foreach (StudentRequest request in fileList.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
        {
            (User user, Dictionary<string, string[]>? errors) =
                await authManager.Register(request, [UserRole.Student]);

            if (errors != null)
            {
                logger.LogWarning("Failed to register student {Email}: {Errors}",
                    request.Email,
                    string.Join(", ", errors.SelectMany(e => e.Value)));
                continue;
            }

            successfulRegistrations.Add((user.Id, request));
        }

        if (successfulRegistrations.Count <= 0) return;

        await repository.CreateBulk(successfulRegistrations, cancellationToken);
    }
}