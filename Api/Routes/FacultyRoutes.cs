using Domain.Dto.Faculty;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes;

public static class FacultyRoutes
{
    public static async Task<Ok<List<FacultyResponse>>> Get(FacultyRepository repository)
    {
        return TypedResults.Ok(await repository.Get());
    }

    public static async Task<Created> Create(FacultyRequest request, FacultyRepository repository)
    {
        await repository.Create(request);
        return TypedResults.Created();
    }

    public static async Task<NoContent> Update(Guid id, FacultyRequest request, FacultyRepository repository)
    {
        await repository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> Delete(Guid id, FacultyRepository repository)
    {
        bool isSuccess = await repository.Delete(id);
        return isSuccess ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound>> Restore(Guid id, FacultyRepository repository)
    {
        bool isSuccess = await repository.Restore(id);
        return isSuccess ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}