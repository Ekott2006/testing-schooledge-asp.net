using Domain.Data;
using Domain.Dto;
using Domain.Dto.Admin;
using Domain.Dto.Student;
using Domain.Helpers;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class AdminRepository(DataContext context)
{
    // TODO:Convert to a service 
    public async Task Create(string userId, CreateAdminRequest request)
    {
        await context.Admins.AddAsync(request.ToAdmin(userId));
        await context.SaveChangesAsync();
    }

    public async Task<AdminResponse?> Get(string userId)
    {
        Admin? admin = await context.Admins.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId);
        return new AdminResponse(admin);
    }
    public async Task<AdminResponse?> Get(Guid id)
    {
        Admin? admin = await context.Admins.Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id);
        return new AdminResponse(admin);
    }
    public async Task<PagedResult<AdminResponse>> GetAll(PagedRequest request)
    {
        IQueryable<AdminResponse> query = context.Admins.AsQueryable().Include(x => x.User).Select(x => new AdminResponse(x));
        return await query.ToPagedListAsync(request.PageNumber, request.PageSize);
    }

    public async Task Update(string userId, UpdateAdminRequest request)
    {
        await context.Admins.Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.PhoneNumber, request.PhoneNumber)
                .SetProperty(p => p.DateOfBirth, request.DateOfBirth));
    }
    public async Task Delete(string userId)
    {
        await context.Students.Where(x => x.UserId == userId).ExecuteDeleteAsync();
    }
    
}