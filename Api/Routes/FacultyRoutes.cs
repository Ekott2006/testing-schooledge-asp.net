using Domain.Dto.Faculty;
using Domain.Repository;

namespace Api.Routes;

public static class FacultyRoutes
{
    public static async Task<IResult> Get(FacultyRepository repository)
    {
        return TypedResults.Ok(await repository.Get());
    }

    public static async Task<IResult> Create(FacultyRequest request, FacultyRepository repository)
    {
        await repository.Create(request);
        return TypedResults.Created();
    }

    public static async Task<IResult> Update(Guid id, FacultyRequest request, FacultyRepository repository)
    {
        await repository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<IResult> Delete(Guid id, FacultyRepository repository)
    {
        await repository.Delete(id);
        return TypedResults.NoContent();
    }

    public static async Task<IResult> Restore(Guid id, FacultyRepository repository)
    {
        await repository.Restore(id);
        return TypedResults.NoContent();
    }
}