using Domain.Model.Helpers;
using Microsoft.AspNetCore.Identity;

namespace Domain.Model;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string ProfileImageUrl { get; set; }
    public List<UserRefreshToken> RefreshTokens { get; set; } = [];
}