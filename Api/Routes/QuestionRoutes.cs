using Api.BackgroundTask;
using Api.Helpers;
using Domain.Dto.Question;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes;

public static class QuestionRoutes
{
    public static async Task<NoContent> Upsert(QuestionRequest request, QuestionRepository repository)
    { 
        await repository.Upsert(request);
        return TypedResults.NoContent();
    } 
    
    public static async Task<Results<NoContent, BadRequest<string>>> BulkUpsert(IFormFile file, QuestionFileProcessor processor)
    {
        (bool success, string message, string? filePath) = await FileHelper.UploadFileAsync(file, FileHelper.FileDocUpload, FileHelper.FileDocExtensions);
        if (!success || filePath == null) return TypedResults.BadRequest(message);
        processor.QueueFile(filePath);
        return TypedResults.NoContent();
    }
    public static async Task<Results<NoContent, NotFound>> Delete(Guid id, QuestionRepository repository)
    { 
        bool delete = await repository.Delete(id);
        return delete == false ? TypedResults.NotFound() : TypedResults.NoContent();
    } 
    public static async Task<Results<NoContent, NotFound>> Restore(Guid id, QuestionRepository repository)
    { 
        bool restore = await repository.Restore(id);
        return restore == false ? TypedResults.NotFound() : TypedResults.NoContent();
    } 
}