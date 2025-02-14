using Bogus;
using Domain.Model;
using Domain.Model.Helpers;

namespace Domain.Seed;

public sealed class ExamFaker : Faker<Exam>
{
    public ExamFaker(Random random,  List<string> academicSessions, Guid adminId, Guid courseId)
    {
        TimeSpan timeRange = TimeSpan.FromDays(30);
        ExamStatus[] examStatusList = Enum.GetValues<ExamStatus>();
        RuleFor(c => c.Title, f => f.Lorem.Sentence());
        RuleFor(e => e.AcademicSession, _ => academicSessions[random.Next(academicSessions.Count)]);
        RuleFor(c => c.Instructions, f => f.Lorem.Sentences());
        RuleFor(c => c.Instructor, f => f.Name.FullName());
        RuleFor(c => c.StartDateTime, f => f.Date.Between(DateTime.Today - timeRange, DateTime.Today + timeRange));
        RuleFor(c => c.Duration, f => f.Date.Timespan(TimeSpan.FromHours(4)));
        RuleFor(c => c.ReleaseDate, f => f.Date.Between(DateTime.Today - timeRange, DateTime.Today + timeRange));
        RuleFor(c => c.MaxAttempts, _ => 1);
        RuleFor(c => c.TotalQuestions, _ => random.Next(5, 10));
        RuleFor(c => c.Status, _ => examStatusList[random.Next(examStatusList.Length)]);
        RuleFor(c => c.AcademicSession, f => f.Company.CompanyName());
        RuleFor(c => c.CreatedById, _ => adminId);
        RuleFor(c => c.CourseId, _ => courseId);
        
    }
}