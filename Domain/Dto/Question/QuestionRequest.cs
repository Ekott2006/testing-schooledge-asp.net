namespace Domain.Dto.Question;

public class QuestionRequest
{
    public Guid ExamId { get; set; }
    public int QuestionNumber { get; set; }
    public string Content { get; set; }
    public ICollection<string> Choices { get; set; } = [];
    public string CorrectAnswer { get; set; }
    public string Explanation { get; set; } = string.Empty;

    public static implicit operator Model.Question(QuestionRequest request) => new()
    {
        ExamId = request.ExamId,
        QuestionNumber = request.QuestionNumber,
        Content = request.Content,
        Choices = request.Choices,
        CorrectAnswer = request.CorrectAnswer,
        Explanation = request.Explanation,
    };
}