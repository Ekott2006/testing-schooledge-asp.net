using System.Security.Claims;
using Api.Helpers;
using Domain.Dto.User;
using Domain.Model;
using Domain.Repository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Routes;

public static class UserRoutes
{
    public static async Task<Results<ChallengeHttpResult, Ok<AuthResponse>>> Refresh(RefreshTokenRequest request, AuthManager authManager,
        ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        User? user = await authManager.DeleteToken(userId, request);
        return user == null ? TypedResults.Challenge() : TypedResults.Ok(await authManager.GenerateToken(user));
    }

    public static async Task<NoContent> ChangePassword(ChangePasswordRequest request, AuthManager authManager,
        ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        await authManager.ChangePassword(userId, request);
        return TypedResults.NoContent();
    }
    
    public static async Task<Results<Ok<string>, BadRequest<string>>> SetProfileImage (IFormFile file, UserRepository repository, ClaimsPrincipal principal)
    {
        (bool success, string message, string? filePath) = await FileHelper.UploadFileAsync(file, FileHelper.FileImageUpload, FileHelper.FileImageExtensions,
            20 * 1024 * 1024);
        if (!success || filePath == null) return TypedResults.BadRequest(message);
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        await repository.UpdateProfileImage(userId, filePath);
        return TypedResults.Ok(filePath);
    }

    public static async Task<NoContent> Logout(RefreshTokenRequest request, AuthManager authManager,
        ClaimsPrincipal principal)
    {
        string userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        await authManager.DeleteToken(userId, request);
        return TypedResults.NoContent();
    }
}
