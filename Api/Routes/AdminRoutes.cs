using System.Security.Claims;
using Domain.Dto;
using Domain.Dto.Admin;
using Domain.Dto.User;
using Domain.Model;
using Domain.Model.Helpers;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes;

public static class AdminRoutes
{
    
    public static async Task<Results<Created, ValidationProblem>> Create(CreateAdminRequest request, AdminRepository repository, AuthManager authManager)
    {
        (User user, Dictionary<string, string[]>? errors) = await authManager.Register(request, [UserRole.Admin]);
        if (errors != null) return TypedResults.ValidationProblem(errors);
        await repository.Create(user.Id, request);
        return TypedResults.Created();
    }
    
    public static async Task<Results<Ok<AuthResponse>, ProblemHttpResult>> Login(LoginWithPasswordRequest request, AuthManager authManager)
    {
        (User? user, string? signInState) = await authManager.LoginWithPassword(request);
        if (signInState != null || user == null)
            return TypedResults.Problem(signInState, statusCode: StatusCodes.Status401Unauthorized);
        return TypedResults.Ok(await authManager.GenerateToken(user));
    }
    
    public static async Task<Results<Ok<Admin>, UnauthorizedHttpResult>> GetProfile(AdminRepository repository, ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        Admin? admin = await repository.Get(userId);
        return admin == null ? TypedResults.Unauthorized() : TypedResults.Ok(admin);
    }
    
    public static async Task<Results<Ok<Admin>, NotFound>> Get(string id, AdminRepository repository)
    {
        Admin? admin = await repository.Get(id);
        return admin == null ? TypedResults.NotFound() : TypedResults.Ok(admin);
    }
    public static async Task<Ok<PagedResult<Admin>>> GetAll([AsParameters] PagedRequest request, AdminRepository repository)
    {
        return TypedResults.Ok(await repository.GetAll(request));
    }
    
    public static async Task<NoContent> Update(string id, UpdateAdminRequest request, UserRepository userRepository, AdminRepository repository)
    {
        await repository.Update(id, request);
        await userRepository.Update(id, request);
        return TypedResults.NoContent();
    }

    public static async Task<NoContent> Delete(string id, AdminRepository repository, AuthManager authManager)
    {
        await repository.Delete(id);
        await authManager.DeleteAccount(id);
        return TypedResults.NoContent();
    }
}