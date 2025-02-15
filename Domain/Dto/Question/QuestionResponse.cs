using Domain.Dto.Exam;

namespace Domain.Dto.Question;

public class QuestionResponse()
{
    public Guid Id { get; set; }
    public int QuestionNumber { get; set; }
    public string Content { get; set; }
    public ICollection<string> Choices { get; set; } = [];
    public string CorrectAnswer { get; set; }
    public string Explanation { get; set; }
    public Guid ExamId { get; set; }
    public QuestionResponse(Model.Question? question) : this()
    {
        if (question == null) return;
        Id = question.Id;
        ExamId = question.Exam.Id;
        QuestionNumber = question.QuestionNumber;
        Content = question.Content;
        Choices = question.Choices;
        CorrectAnswer = question.CorrectAnswer;
        Explanation = question.Explanation;
    }

    
}