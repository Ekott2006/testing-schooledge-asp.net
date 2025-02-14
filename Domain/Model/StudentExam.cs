using Domain.Model.Helpers;

namespace Domain.Model;

public class StudentExam
{
    public Guid Id { get; set; }
    public Guid ExamId { get; set; }
    public Exam Exam { get; set; }
    public Guid StudentId { get; set; }
    public Student Student { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public StudentExamStatus Status { get; set; } = StudentExamStatus.NotStarted;

    // ✅ Auto-Grading Fields
    public int TotalScore { get; set; } = 0;
    public int CorrectAnswers { get; set; } = 0;

    public List<StudentAnswer> StudentAnswers { get; set; } = [];
}