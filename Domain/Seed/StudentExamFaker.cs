using Bogus;
using Domain.Model;
using Domain.Model.Helpers;

namespace Domain.Seed;

public sealed class StudentExamFaker : Faker<StudentExam>
{
    // TODO: Generate the student exam well 
    public StudentExamFaker(Faker faker, Guid studentId, Exam exam)
    {
        DateTime dateTime = faker.Date.Between(exam.StartDateTime - TimeSpan.FromDays(10), exam.StartDateTime + TimeSpan.FromDays(10));
        DateTime now = DateTime.Now;
        StudentExamStatus status = dateTime < now ? StudentExamStatus.NotStarted : StudentExamStatus.Completed;
        DateTime startTime = dateTime < now ? DateTime.Now.AddYears(5) : dateTime;
        
        
        // RuleFor(a => a.Answer, _ => answer);
        RuleFor(a => a.ExamId, _ => exam.Id);
        RuleFor(a => a.StudentId, _ => studentId);
        RuleFor(a => a.StartTime, _ => startTime);
        RuleFor(a => a.EndTime, _ => startTime + exam.Duration);
        RuleFor(a => a.Status, _ => status);
        
    }
}