using Bogus;
using Domain.Model;

namespace Domain.Seed;

public sealed class FacultyFaker : Faker<Faculty>
{
    public FacultyFaker()
    {
        RuleFor(c => c.Name, f => f.Lorem.Word());
    }
}