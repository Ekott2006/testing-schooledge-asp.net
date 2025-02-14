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

    public async Task<Admin?> Get(string userId)
    {
        return await context.Admins.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId);
    }
    public async Task<PagedResult<Admin>> GetAll(PagedRequest request)
    {
        IQueryable<Admin> query = context.Admins.AsQueryable().Include(x => x.User);
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