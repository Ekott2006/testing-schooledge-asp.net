namespace Domain.Dto.Institution;

public class CreateInstitutionRequest : InstitutionRequest
{
    public string LogoUrl { get; set; }

    public static implicit operator Model.Institution(CreateInstitutionRequest request) => new()
    {
        Name = request.Name,
        Code = request.Code,
        CurrentAcademicSession = request.CurrentAcademicSession,
        LogoUrl = request.LogoUrl,
        Type = request.Type,
        IsResultVisibleAfterTest = request.IsResultVisibleAfterTest
    };
}