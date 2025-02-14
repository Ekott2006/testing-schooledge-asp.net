using Domain.Data;
using Domain.Dto.Institution;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class InstitutionRepository(DataContext context)
{
    public async Task<Institution?> Get() => await context.Institutions.FirstOrDefaultAsync();

    public async Task Create(CreateInstitutionRequest request)
    {
        await context.Institutions.AddAsync(request);
        await context.SaveChangesAsync();
    }

    public async Task Update(Guid id, InstitutionRequest request)
    {
        await context.Institutions
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.Name, request.Name)
                    .SetProperty(x => x.Code, request.Code)
                    .SetProperty(x => x.CurrentAcademicSession, request.CurrentAcademicSession)
                    .SetProperty(x => x.Type, request.Type)
                    .SetProperty(x => x.IsResultVisibleAfterTest, request.IsResultVisibleAfterTest)
            );
    }

    public async Task UpdateProfileImage(Guid id, string logoUrl)
    {
        await context.Institutions
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(x => x.LogoUrl, logoUrl)
            );
    }
}