using Domain.Dto.User;
using Domain.Model.Helpers;

namespace Domain.Dto.Student;

public class StudentRequest : RegisterRequest, IStudentRequest
{

    public Model.Student ToStudent(string userId, ICollection<Model.Course> courses) => new()
    {
        UserId = userId,
        Level = Level,
        DepartmentId = DepartmentId,
        Courses = courses
    };

    public Guid DepartmentId { get; set; }
    public CourseLevel Level { get; set; }
}