using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using Domain.Data;
using Domain.Dto.User;
using Domain.Helpers;
using Domain.Model;
using Domain.Model.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Domain.Repository;

public class AuthManager(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    DataContext context,
    IConfiguration configuration)
{
    private const string NoPassword = "k£R6b#h1f[1£";

    public async Task<(User user, Dictionary<string, string[]>? errors)> Register(RegisterRequest request, IEnumerable<UserRole> rolesList)
    {
        return await Register(new RegisterWithPasswordRequest
        {
            UserName = request.UserName,
            Email = request.Email,
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Password = NoPassword,
            ProfileImageUrl = request.ProfileImageUrl
        }, rolesList);
    }

    public async Task<(User user, Dictionary<string, string[]>? errors)> Register(RegisterWithPasswordRequest request,
        IEnumerable<UserRole> rolesList)
    {
        User user = new()
        {
            UserName = request.UserName, Email = request.Email, FirstName = request.FirstName,
            LastName = request.LastName, MiddleName = request.MiddleName, ProfileImageUrl = request.ProfileImageUrl
        };
        IdentityResult result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) return (user, CreateValidationProblem(result));
        await userManager.AddToRolesAsync(user, rolesList.Select(role => role.ToString()));
        return (user, null);
    }

    public async Task<(User? user, string? signInState)> Login(LoginRequest request)
    {
        User? user = await userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return (user, SignInResult.Failed.ToString());
        await signInManager.SignInAsync(user, false);
        return (user, null);
    }
    public async Task<(User? user, string? signInState)> LoginWithPassword(LoginWithPasswordRequest request)
    {
        User? user = await userManager.FindByNameAsync(request.UserName);
        if (user == null)
            return (user, SignInResult.Failed.ToString());
        SignInResult attempt = await signInManager.PasswordSignInAsync(user, request.Password, false, true);
        return (user, !attempt.Succeeded ? attempt.ToString() : null);
    }

    public async Task<AuthResponse> GenerateToken(User user)
    {
        // Handling RefreshTokens
        UserRefreshToken refreshToken = new()
        {
            UserId = user.Id, Validity = DateTime.Now.AddDays(TokenHelper.RefreshTokenValidityInDays),
            Token = TokenHelper.GenerateRefreshToken()
        };
        await context.UserRefreshTokens.AddAsync(refreshToken);
        user.RefreshTokens.Add(refreshToken);
        await context.SaveChangesAsync();

        IList<string> roles = await userManager.GetRolesAsync(user);
        return new AuthResponse(user.Id, TokenHelper.CreateAccessToken(configuration, user, roles.ToList()),
            refreshToken.Token);
    }

    public async Task<User?> DeleteToken(string id, RefreshTokenRequest dto)
    {
        User? user = await context.Users.Include(x => x.RefreshTokens)
            .FirstOrDefaultAsync(user1 => user1.Id == id);
        UserRefreshToken? userRefreshToken = user?.RefreshTokens.FirstOrDefault(tokens =>
            tokens.Token == dto.RefreshToken && DateTime.Now <= tokens.Validity);
        if (userRefreshToken == null) return null;
        context.UserRefreshTokens.Remove(userRefreshToken);
        await context.SaveChangesAsync();
        return user;
    }

    public async Task<Dictionary<string, string[]>> ChangePassword(string id, ChangePasswordRequest request)
    {
        User? user = await userManager.FindByIdAsync(id);

        if (user is null)
            return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
        IdentityResult result =
            await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return !result.Succeeded ? CreateValidationProblem(result) : new Dictionary<string, string[]>();
    }

    public async Task<User?> DeleteAccount(string id)
    {
        User? user = await userManager.FindByIdAsync(id);
        if (user == null) return null;
        await userManager.DeleteAsync(user);
        return user;
    }

    private static Dictionary<string, string[]> CreateValidationProblem(IdentityResult result)
    {
        Debug.Assert(!result.Succeeded);
        Dictionary<string, string[]> errorDictionary = new(1);

        foreach (IdentityError error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out string[]? descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return errorDictionary;
    }
}