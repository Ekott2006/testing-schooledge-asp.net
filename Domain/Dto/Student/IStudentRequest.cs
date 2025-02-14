using Domain.Model.Helpers;

namespace Domain.Dto.Student;

public interface IStudentRequest
{
    public Guid DepartmentId { get; set; }
    public CourseLevel Level { get; set; }
}