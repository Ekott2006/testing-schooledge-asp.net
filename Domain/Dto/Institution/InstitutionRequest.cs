using Domain.Model.Helpers;

namespace Domain.Dto.Institution;

public class InstitutionRequest
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string CurrentAcademicSession { get; set; }
    public InstitutionType Type { get; set; }
    public bool IsResultVisibleAfterTest { get; set; } = false;
}