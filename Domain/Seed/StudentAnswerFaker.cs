using Bogus;
using Domain.Model;

namespace Domain.Seed;

public sealed class StudentAnswerFaker : Faker<StudentAnswer>
{
    public StudentAnswerFaker(Guid studentExamId, Guid questionId, string answer)
    {
        RuleFor(a => a.Answer, _ => answer);
        RuleFor(a => a.StudentExamId, _ => studentExamId);
        RuleFor(a => a.QuestionId, _ => questionId);
    }
}