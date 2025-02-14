namespace ClassLibrary1;

public class StudentExam
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string StudentId { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public StudentExamStatus Status { get; set; } = StudentExamStatus.NotStarted;

    // ✅ Auto-Grading Fields
    public int TotalScore { get; set; } = 0;
    public int CorrectAnswers { get; set; } = 0;
    

    public Exam Exam { get; set; }
    public List<StudentAnswer> StudentAnswers { get; set; } = new();
}