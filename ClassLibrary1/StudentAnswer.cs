namespace ClassLibrary1;

public class StudentAnswer
{
    public int Id { get; set; }
    public string Answer { get; set; } = string.Empty;

    // Navigation Property
    public int StudentExamId { get; set; }
    public int QuestionId { get; set; }
    public StudentExam StudentExam { get; set; }
    public Question Question { get; set; }
}