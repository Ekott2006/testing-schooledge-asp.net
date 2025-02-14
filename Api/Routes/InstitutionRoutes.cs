using System.Security.Claims;
using Api.Helpers;
using Domain.Dto.Institution;
using Domain.Model;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes;

public static class InstitutionRoutes
{
    public static async Task<Results<Ok<Institution>, NotFound>> Get(InstitutionRepository repository)
    {
        Institution? institution = await repository.Get();
        return institution == null ? TypedResults.NotFound() : TypedResults.Ok(institution);
    }

    public static async Task<Created> Create(CreateInstitutionRequest request, InstitutionRepository repository)
    {
        await repository.Create(request);
        return TypedResults.Created();
    }
    public static async Task<NoContent> Update(Guid id, InstitutionRequest request, InstitutionRepository repository)
    {
        await repository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<Results<Ok<string>, BadRequest<string>>> SetProfileImage (Guid id, IFormFile file, InstitutionRepository repository)
    {
        (bool success, string message, string? filePath) = await FileHelper.UploadFileAsync(file, FileHelper.FileImageUpload, FileHelper.FileImageExtensions,
            20 * 1024 * 1024);
        if (!success || filePath == null) return TypedResults.BadRequest(message);
        await repository.UpdateProfileImage(id, filePath);
        return TypedResults.Ok(filePath);
    }
}