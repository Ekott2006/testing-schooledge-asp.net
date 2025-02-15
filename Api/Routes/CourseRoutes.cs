using Domain.Dto.Course;
using Domain.Model;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes;

public static class CourseRoutes
{
    public static async Task<Ok<List<CourseResponse>>> Get(CourseRepository repository)
    {
        return TypedResults.Ok(await repository.Get());
    }

    public static async Task<Created> Create(CourseRequest request, CourseRepository repository)
    {
        await repository.Create(request);
        return TypedResults.Created();
    }

    public static async Task<NoContent> Update(Guid id, CourseRequest request, CourseRepository repository)
    {
        await repository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> Delete(Guid id, CourseRepository repository)
    {
        bool delete = await repository.Delete(id);
        return  delete ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound>> Restore(Guid id, CourseRepository repository)
    {
        bool restore = await repository.Restore(id);
        return  restore ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}