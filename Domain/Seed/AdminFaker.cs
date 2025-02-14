using Bogus;
using Domain.Model;

namespace Domain.Seed;

public sealed class AdminFaker : Faker<Admin>
{
    public AdminFaker(string userId)
    {
        RuleFor(a => a.DateOfBirth, faker => faker.Date.Past());
        RuleFor(a => a.PhoneNumber, faker => faker.Phone.PhoneNumber());
        RuleFor(a => a.UserId, _ => userId);
    }
}