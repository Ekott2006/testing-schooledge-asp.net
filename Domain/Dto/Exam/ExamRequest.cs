using Domain.Model.Helpers;

namespace Domain.Dto.Exam;

public class ExamRequest
{
    public Guid CourseId { get; set; }
    public string Title { get; set; }
    public string Instructions { get; set; }
    public string Instructor { get; set; }
    public DateTime StartDateTime { get; set; }
    public TimeSpan Duration { get; set; }
    public int MaxAttempts { get; set; } 
    public decimal PointsPerQuestion { get; set; }
    public int TotalQuestions { get; set; }
    public string AcademicSession { get; set; }
    public DateTime? ReleaseDate { get; set; }
    public Model.Exam ToExam(Guid studentId) => new()
    {
        Title = Title,
        AcademicSession = AcademicSession,
        Instructor = Instructor,
        Instructions = Instructions,
        StartDateTime = StartDateTime,
        Duration = Duration,
        MaxAttempts = MaxAttempts,
        TotalQuestions = TotalQuestions,
        PointsPerQuestion = PointsPerQuestion,
        Status = ExamStatus.NotStarted,
        CourseId = CourseId,
        CreatedById = studentId,
        ReleaseDate = ReleaseDate ?? DateTime.Now 
    };
}