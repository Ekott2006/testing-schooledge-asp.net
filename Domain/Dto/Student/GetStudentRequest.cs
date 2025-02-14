using Domain.Model.Helpers;

namespace Domain.Dto.Student;

public class GetStudentRequest: PagedRequest
{
    public string? Search { get; set; }
    public Guid? FacultyId { get; set; }
    public Guid? DepartmentId { get; set; }
    public CourseLevel? Level { get; set; }
}