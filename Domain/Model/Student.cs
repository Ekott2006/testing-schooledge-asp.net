using Domain.Model.Helpers;

namespace Domain.Model;

public class Student: ISoftDeletable
{
    public Guid Id { get; set; }
    public string UserId { get; set; }  
    public User User { get; set; }
    public CourseLevel Level { get; set; }
    public Guid DepartmentId { get; set; }
    public Department Department { get; set; }
    public ICollection<Course> Courses { get; set; } = [];
    public bool IsDeleted { get; set; }
}