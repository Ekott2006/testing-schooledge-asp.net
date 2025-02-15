using Domain.Dto.Admin;
using Domain.Dto.Course;
using Domain.Dto.Question;
using Domain.Model.Helpers;

namespace Domain.Dto.Exam;

public class ExamResponse()
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
    public Guid? CreatedById { get; set; }
    public ICollection<QuestionResponse> Questions { get; set; } = [];
    public CourseResponse? Course { get; set; }
    public decimal OverallScore => PointsPerQuestion * TotalQuestions;

    public ExamResponse(Model.Exam? exam) : this()
    {
        if (exam == null) return;
        Id = exam.Id;
        Title = exam.Title;
        AcademicSession = exam.AcademicSession;
        Instructor = exam.Instructor;
        Instructions = exam.Instructions;
        StartDateTime = exam.StartDateTime;
        Duration = exam.Duration;
        ReleaseDate = exam.ReleaseDate;
        MaxAttempts = exam.MaxAttempts;
        TotalQuestions = exam.TotalQuestions;
        PointsPerQuestion = exam.PointsPerQuestion;
        Status = exam.Status;
        CreatedById = exam.CreatedById;
        Course = new CourseResponse(exam.Course);
        Questions = exam.Questions.Select(x => new QuestionResponse(x)).ToList();
    }
}