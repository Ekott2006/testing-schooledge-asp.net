using Bogus;
using Domain.Model;

namespace Domain.Seed;

public sealed class DepartmentFaker : Faker<Department>
{
    public DepartmentFaker(Guid facultyId)
    {
        RuleFor(c => c.Name, f => f.Lorem.Sentence());
        RuleFor(c => c.FacultyId, _ => facultyId);
    }
}