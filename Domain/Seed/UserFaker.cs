using Bogus;
using Domain.Model;

namespace Domain.Seed;

public sealed class UserFaker : Faker<User>
{
    public UserFaker(string userName)
    {
        RuleFor(c => c.Email, f => f.Internet.Email());
        RuleFor(c => c.UserName, _ => userName);
        RuleFor(c => c.FirstName, f => f.Name.FirstName());
        RuleFor(c => c.MiddleName, f => f.Name.LastName());
        RuleFor(c => c.LastName, f => f.Name.LastName());
        RuleFor(c => c.ProfileImageUrl, _ => "https://avatar.iran.liara.run/public");
    }
}