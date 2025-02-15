using Domain.Dto.Department;

namespace Domain.Dto.Faculty;

public class FacultyResponse()
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public ICollection<DepartmentResponse> Departments { get; set; } = [];

    public FacultyResponse(Model.Faculty? faculty) : this()
    {
        if (faculty == null) return;
        Id = faculty.Id;
        Name = faculty.Name;
        Departments = faculty.Departments.Select(x => new DepartmentResponse(x)).ToList();
    }
}