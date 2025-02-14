using Domain.Model;
using Domain.Model.Helpers;

namespace Domain.Dto.Course;

public class CourseRequest
{
    public string Name { get; set; }
    public string Code { get; set; }
    public CourseLevel Level { get; set; }
    public Guid DepartmentId { get; set; }
    
    public Model.Course ToCourse(ICollection<Model.Student> students) => new()
    {
        Name = Name,
        CourseCode = Code,
        Level = Level,
        DepartmentId = DepartmentId,
        Students =  students
    };
}