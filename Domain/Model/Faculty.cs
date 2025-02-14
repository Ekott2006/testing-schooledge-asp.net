using Domain.Model.Helpers;

namespace Domain.Model;

public class Faculty : ISoftDeletable
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<Department> Departments { get; set; } = [];
}