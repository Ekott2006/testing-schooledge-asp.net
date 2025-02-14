namespace Domain.Dto.Faculty;

public class FacultyRequest
{
    public string Name { get; set; }

    public static implicit operator Model.Faculty(FacultyRequest request) => new()
    {
        Name = request.Name,
    };
}