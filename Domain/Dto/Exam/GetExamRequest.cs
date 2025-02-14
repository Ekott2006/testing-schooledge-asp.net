using Domain.Model.Helpers;

namespace Domain.Dto.Exam;

public class GetExamRequest : PagedRequest
{
    public ExamStatus? Status { get; set; }
    public string? Title { get; set; }
    public Guid? FacultyId { get; set; }
    public Guid? DepartmentId { get; set; }
    public CourseLevel? Level { get; set; }
}