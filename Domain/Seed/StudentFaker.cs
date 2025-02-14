using Bogus;
using Domain.Model;
using Domain.Model.Helpers;

namespace Domain.Seed;

public sealed class StudentFaker : Faker<Student>
{
    public StudentFaker(Random random, string userId, Guid departmentId)
    {
        CourseLevel[] courseLevels = Enum.GetValues<CourseLevel>();
        RuleFor(a => a.UserId, _ => userId);
        RuleFor(c => c.Level, _ => courseLevels[random.Next(courseLevels.Length)]);
        RuleFor(a => a.DepartmentId, _ => departmentId);
    }
}