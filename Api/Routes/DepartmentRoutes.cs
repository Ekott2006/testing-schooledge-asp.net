using Domain.Dto.Department;
using Domain.Repository;

namespace Api.Routes;

public static class DepartmentRoutes
{
    public static async Task<IResult> Get(DepartmentRepository repository)
    {
        return TypedResults.Ok(await repository.Get());
    }

    public static async Task<IResult> Create(DepartmentRequest request, DepartmentRepository repository)
    {
        await repository.Create(request);
        return TypedResults.Created();
    }

    public static async Task<IResult> Update(Guid id, DepartmentRequest request, DepartmentRepository repository)
    {
        await repository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<IResult> Delete(Guid id, DepartmentRepository repository)
    {
        await repository.Delete(id);
        return TypedResults.NoContent();
    }public static async Task<IResult> Restore(Guid id, DepartmentRepository repository)
    {
        await repository.Restore(id);
        return TypedResults.NoContent();
    }
}