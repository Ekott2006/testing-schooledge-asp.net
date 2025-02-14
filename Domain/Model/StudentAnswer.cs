using Domain.Model.Helpers;

namespace Domain.Model;

public class StudentAnswer
{
    public Guid Id { get; set; }
    public string Answer { get; set; }

    // Navigation Property
    public Guid StudentExamId { get; set; }
    public StudentExam StudentExam { get; set; }
    public Guid QuestionId { get; set; }
    public Question Question { get; set; }
}