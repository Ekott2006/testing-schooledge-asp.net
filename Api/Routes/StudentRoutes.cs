using System.Security.Claims;
using Api.BackgroundTask;
using Api.Helpers;
using Domain.Dto.Student;
using Domain.Dto.User;
using Domain.Model;
using Domain.Model.Helpers;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes;

public static class StudentRoutes
{
    public static async Task<Results<ValidationProblem, Created>> Create(StudentRequest request, StudentRepository repository, AuthManager authManager)
    {
        (User user, Dictionary<string, string[]>? errors) = await authManager.Register(request, [UserRole.Student]);
        if (errors != null) return TypedResults.ValidationProblem(errors);
        await repository.Create(user.Id, request);
        return TypedResults.Created();
    }
    
    public static async Task<Results<NoContent, BadRequest<string>>> BulkCreate(IFormFile file, StudentFileProcessor backgroundTask)
    {
        (bool success, string message, string? filePath) = await FileHelper.UploadFileAsync(file, FileHelper.FileDocUpload, FileHelper.FileDocExtensions);
        if (!success || filePath == null) return TypedResults.BadRequest(message);
        backgroundTask.QueueFile(filePath);
        return TypedResults.NoContent();
    }

    public static async Task<IResult> Login(LoginRequest request, AuthManager authManager)
    {
        (User? user, string? signInState) = await authManager.Login(request);
        if (signInState != null || user == null)
            return TypedResults.Problem(signInState, statusCode: StatusCodes.Status401Unauthorized);
        return TypedResults.Ok(await authManager.GenerateToken(user));
    }

    public static async Task<IResult> GetProfile(StudentRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        Student? student = await repository.Get(userId);
        return TypedResults.Ok(student);
    }

    public static async Task<IResult> Get(string id, StudentRepository repository)
    {
        return TypedResults.Ok(await repository.Get(id));
    }

    public static async Task<IResult> GetAll([AsParameters] GetStudentRequest request, [FromServices] StudentRepository repository)
    {
        return TypedResults.Ok(await repository.GetAll(request));
    }
    
    // TODO: Make it One Route
    public static async Task<Ok<List<Exam>>> GetAvailableExams(StudentRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        return TypedResults.Ok(await repository.GetAvailableExams(userId));
    }

    public static async Task<Ok<List<Exam>>> GetUpcomingExams(StudentRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        return TypedResults.Ok(await repository.GetUpcomingExams(userId));
    }

    public static async Task<IResult> Update(string id, UpdateStudentRequest request, UserRepository userRepository, StudentRepository repository)
    {
        await repository.Update(id, request);
        await userRepository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<IResult> Delete(string id, StudentRepository repository, AuthManager authManager)
    {
        await repository.Delete(id);
        await authManager.DeleteAccount(id);
        return TypedResults.NoContent();
    }
}