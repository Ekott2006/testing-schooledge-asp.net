using Domain.Data;
using Domain.Dto.Faculty;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class FacultyRepository(DataContext context)
{
    public async Task<List<FacultyResponse>> Get(bool? isDeleted = false)
    {
        List<Faculty> result =  isDeleted switch
        {
            true => await context.Faculties.IgnoreQueryFilters().Where(x => x.IsDeleted).ToListAsync(),
            false => await context.Faculties.ToListAsync(),
            _ => await context.Faculties.IgnoreQueryFilters().ToListAsync()
        };
        return result.Select(x => new FacultyResponse(x)).ToList();
    } 
    
    public async Task<bool> Restore(Guid id)
    {
        Faculty? faculty = await context.Faculties.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id);

        if (faculty == null) return false;
        faculty.IsDeleted = false;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task Create(FacultyRequest request)
    {
        await context.Faculties.AddAsync(request);
        await context.SaveChangesAsync();
    }

    public async Task Update(Guid id, FacultyRequest request)
    {
        await context.Faculties.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(faculty => faculty.Name, request.Name));
    }

    public async Task<bool> Delete(Guid id)
    {
        Faculty? faculty = await context.Faculties.SingleOrDefaultAsync(x => x.Id == id);
        if (faculty == null) return false;
        context.Faculties.Remove(faculty);
        await context.SaveChangesAsync();
        return true;
    }
}