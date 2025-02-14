namespace ClassLibrary1;

public class Question
{
    public Question()
    {
    }

    public Question(string text, string correctAnswer)
    {
        Text = text;
        CorrectAnswer = correctAnswer;
    }

    public int Id { get; set; }
    
    public string Text { get; set; } = string.Empty;
    public string CorrectAnswer { get; set; } = string.Empty;

    // Navigation Property
    public int ExamId { get; set; }
    public Exam Exam { get; set; }
}