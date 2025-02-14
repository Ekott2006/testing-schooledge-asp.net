using Domain.Model.Helpers;

namespace Domain.Model;

public class Department : ISoftDeletable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public Guid FacultyId { get; set; }
    public Faculty Faculty { get; set; }
    public ICollection<Course> Courses { get; set; } = [];
}