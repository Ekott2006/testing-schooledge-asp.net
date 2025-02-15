using System.Security.Claims;
using Domain.Dto;
using Domain.Dto.Exam;
using Domain.Model;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes;

public static class ExamRoutes
{
    
    public static async Task<Results<Ok<ExamResponse>, NotFound>> Get(Guid id, ExamRepository repository)
    {
        ExamResponse? exam = await repository.Get(id);
        return exam == null ? TypedResults.NotFound() : TypedResults.Ok(exam);
    }

    public static async Task<Ok<PagedResult<ExamResponse>>> GetAll([AsParameters] GetExamRequest request,
        ExamRepository repository)
    {
        return TypedResults.Ok(await repository.Get(request));
    }
    
    public static async Task<Created> Create(ExamRequest request, ExamRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        await repository.Create(userId, request);
        return TypedResults.Created();
    }

    public static async Task<NoContent> Update(Guid id, ExamRequest request, ExamRepository repository)
    {
        await repository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> Delete(Guid id, ExamRepository repository)
    {
        await repository.Delete(id);
        return TypedResults.NoContent();
    }
    public static async Task<NoContent> Restore(Guid id, ExamRepository repository)
    {
        await repository.Restore(id);
        return TypedResults.NoContent();
    }
}