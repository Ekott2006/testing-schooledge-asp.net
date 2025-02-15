using Domain.Dto.Course;
using Domain.Dto.Department;
using Domain.Dto.User;
using Domain.Model.Helpers;

namespace Domain.Dto.Student;

public class StudentResponse(): UserResponse
{
    public Guid StudentId { get; set; }
    public CourseLevel Level { get; set; }
    public Guid DepartmentId { get; set; }
    public ICollection<CourseResponse> Courses { get; set; } = [];
    public StudentResponse(Model.Student? student) : this()
    {
        if (student == null) return;
        StudentId = student.Id;
        UserId = student.UserId;
        UserName = student.User?.UserName;
        Email = student.User?.Email;
        FirstName = student.User?.FirstName;
        MiddleName = student.User?.MiddleName;
        LastName = student.User?.LastName;
        ProfileImageUrl = student.User?.ProfileImageUrl;
        DepartmentId = student.DepartmentId;
        Level = student.Level;
        Courses = student.Courses.Select(x => new CourseResponse(x)).ToList();
    }
}
