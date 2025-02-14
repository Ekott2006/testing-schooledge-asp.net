using Domain.Model.Helpers;

namespace Domain.Model;

public class Exam : ISoftDeletable
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string AcademicSession { get; set; }
    public string Instructor { get; set; }
    public string Instructions { get; set; }
    public DateTime StartDateTime { get; set; }
    public TimeSpan Duration { get; set; }
    public DateTime ReleaseDate { get; set; }
    public int MaxAttempts { get; set; } = 1;
    public int TotalQuestions { get; set; }
    public decimal PointsPerQuestion { get; set; } = 1;
    public ExamStatus Status { get; set; }
    public Guid CreatedById { get; set; }
    public Admin CreatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Question> Questions { get; set; } = [];
    public Guid CourseId { get; set; }
    public Course Course { get; set; }
    public decimal OverallScore => PointsPerQuestion * TotalQuestions;
}