using Domain.Dto.User;
using Domain.Model.Helpers;

namespace Domain.Dto.Student;

public class UpdateStudentRequest : UpdateUserRequest, IStudentRequest
{
    public Guid DepartmentId { get; set; }
    public CourseLevel Level { get; set; }
}