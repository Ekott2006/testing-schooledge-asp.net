namespace Domain.Dto.Department;

public class DepartmentRequest
{
    public Guid FacultyId { get; set; }
    public string Name { get; set; }

    public static implicit operator Model.Department(DepartmentRequest request) => new () {
        Name = request.Name,
        FacultyId = request.FacultyId,
    };
}