using Domain.Data;
using Domain.Dto.Department;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class DepartmentRepository(DataContext context)
{
    public async Task<List<DepartmentResponse>> Get(bool? isDeleted = false)
    {
        List<Department> results = isDeleted switch
        {
            true => await context.Departments.IgnoreQueryFilters().Where(x => x.IsDeleted).ToListAsync(),
            false => await context.Departments.ToListAsync(),
            _ => await context.Departments.IgnoreQueryFilters().ToListAsync()
        };
        return results.Select(x => new DepartmentResponse(x)).ToList();
    } 
    
    public async Task<bool> Restore(Guid id)
    {
        Department? department = await context.Departments.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id);

        if (department == null) return false;
        department.IsDeleted = false;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task Create(DepartmentRequest request)
    {
        await context.Departments.AddAsync(request);
        await context.SaveChangesAsync();
    }

    public async Task Update(Guid id, DepartmentRequest request)
    {
        await context.Departments.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(department => department.Name, request.Name));
    }

    public async Task<bool> Delete(Guid id)
    {
        Department? department = await context.Departments.SingleOrDefaultAsync(x => x.Id == id);
        if (department == null) return false;
        context.Departments.Remove(department);
        await context.SaveChangesAsync();
        return true;
    }
}