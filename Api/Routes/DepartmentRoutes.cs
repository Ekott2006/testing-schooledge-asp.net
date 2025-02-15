using Domain.Dto.Department;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Routes;

public static class DepartmentRoutes
{
    public static async Task<Ok<List<DepartmentResponse>>> Get(DepartmentRepository repository)
    {
        return TypedResults.Ok(await repository.Get());
    }

    public static async Task<Created> Create(DepartmentRequest request, DepartmentRepository repository)
    {
        await repository.Create(request);
        return TypedResults.Created();
    }

    public static async Task<NoContent> Update(Guid id, DepartmentRequest request, DepartmentRepository repository)
    {
        await repository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound>> Delete(Guid id, DepartmentRepository repository)
    {
        bool isSuccess = await repository.Delete(id);
        return isSuccess ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    public static async Task<Results<NoContent, NotFound>> Restore(Guid id, DepartmentRepository repository)
    {
        bool isSuccess = await repository.Restore(id);
        return isSuccess ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}