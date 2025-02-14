using Domain.Model.Helpers;

namespace Domain.Model;

public class Question : DateTimeHelper, ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; }
    public int QuestionNumber { get; set; }
    public string Content { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<string> Choices { get; set; } = [];
    public string CorrectAnswer { get; set; }
    public string Explanation { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}