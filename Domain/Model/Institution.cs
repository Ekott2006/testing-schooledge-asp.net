using Domain.Model.Helpers;

namespace Domain.Model;

public class Institution
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string CurrentAcademicSession { get; set; }
    public string LogoUrl { get; set; }
    public InstitutionType Type { get; set; }
    public bool IsResultVisibleAfterTest { get; set; } = false;
}