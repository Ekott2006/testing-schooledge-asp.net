using System.Security.Claims;
using Domain.Dto.StudentExam;
using Domain.Model;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes;

public static class StudentExamRoutes
{
    public static async Task<Ok<List<StudentExamResponse>>> GetAll(StudentExamRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        return TypedResults.Ok(await repository.GetStudentExams(userId));
    }

    public static async Task<Results<Ok<StudentExamResponse>, NotFound>> Get(Guid id, StudentExamRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        StudentExamResponse? studentExam = await repository.GetStudentExamById(userId, id);
        return studentExam == null ? TypedResults.NotFound() : TypedResults.Ok(studentExam);
    }
    public static async Task<Results<Ok<StudentExamResultResponse>, NotFound>> GetResults(Guid id, StudentExamRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        StudentExamResultResponse? examResults = await repository.GetExamResults(userId, id);
        return examResults == null ? TypedResults.NotFound() : TypedResults.Ok(examResults);
    }
    public static async Task<Results<Ok<StudentExamResponse>, NotFound>> Start(Guid examId, StudentExamRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        StudentExamResponse? studentExam = await repository.StartExam(userId, examId);
        return studentExam == null ? TypedResults.NotFound() : TypedResults.Ok(studentExam);
    }

    public static async Task<NoContent> Submit(Guid id, StudentExamRepository repository,
        ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        await repository.SubmitExam(userId, id);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> SubmitAnswer(SubmitStudentExamAnswerRequest request,
        StudentExamRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        await repository.SubmitAnswer(userId, request);
        return TypedResults.NoContent();
    }
    
    
}