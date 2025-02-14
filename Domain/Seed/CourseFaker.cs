using Bogus;
using Domain.Model;
using Domain.Model.Helpers;

namespace Domain.Seed;

public sealed class CourseFaker: Faker<Course>
{
    public CourseFaker(Random random, Guid departmentId)
    {
        CourseLevel[] courseLevels = Enum.GetValues<CourseLevel>();
        RuleFor(c => c.Name, f => f.Lorem.Sentence());
        RuleFor(c => c.CourseCode, f => f.Internet.UserName());
        RuleFor(c => c.Level, _ => courseLevels[random.Next(courseLevels.Length)]);
        RuleFor(c => c.DepartmentId, _ => departmentId);
    }
}