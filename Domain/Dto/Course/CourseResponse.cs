using Domain.Dto.Department;
using Domain.Dto.Exam;
using Domain.Dto.Student;
using Domain.Model.Helpers;

namespace Domain.Dto.Course;

public class CourseResponse()
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CourseCode { get; set; }
    public CourseLevel Level { get; set; }
    public Guid? DepartmentId { get; set; }
    public ICollection<ExamResponse> Exams { get; set; } = [];
    public ICollection<StudentResponse> Students { get; set; } = [];

    public CourseResponse(Model.Course? course) : this()
    {
        if (course == null) return;
        Id = course.Id;
        Name = course.Name;
        CourseCode = course.CourseCode;
        Level = course.Level;
        DepartmentId = course.DepartmentId;
        Students = course.Students.Select(x => new StudentResponse(x)).ToList();
        Exams = course.Exams.Select(x => new ExamResponse(x)).ToList();
    }
}