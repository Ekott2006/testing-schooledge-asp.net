using Domain.Model.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Domain.Model;

// [Index(nameof(CourseCode), IsUnique = true)]
public class Course : ISoftDeletable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string CourseCode { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsDeleted { get; set; }
    public Guid DepartmentId { get; set; }   
    public Department Department { get; set; }
    public ICollection<Exam> Exams { get; set; } = [];
    public ICollection<Student> Students { get; set; } = [];
}