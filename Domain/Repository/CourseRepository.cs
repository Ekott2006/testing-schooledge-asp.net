using Domain.Data;
using Domain.Dto.Course;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class CourseRepository(DataContext context)
{
    public async Task<List<Course>> Get(bool? isDeleted = false)
    {
        return isDeleted switch
        {
            true => await context.Courses.IgnoreQueryFilters().Where(x => x.IsDeleted).ToListAsync(),
            false => await context.Courses.ToListAsync(),
            _ => await context.Courses.IgnoreQueryFilters().ToListAsync()
        };
    }

    public async Task<bool> Restore(Guid id)
    {
        Course? course = await context.Courses.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id);
        if (course == null) return false;
        course.IsDeleted = false;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Create(CourseRequest request)
    {
        Department? department = await context.Departments.SingleOrDefaultAsync(x => x.Id == request.DepartmentId);
        if (department == null) return false;
        ICollection<Student> students = await context.Students.Where(x => x.DepartmentId == request.DepartmentId && x.Level == request.Level).ToListAsync();
        await context.Courses.AddAsync(request.ToCourse(students));
        await context.SaveChangesAsync();
        return true;
    }

    public async Task Update(Guid id, CourseRequest request)
    {
        await context.Courses.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x.SetProperty(course => course.Name, request.Name));
    }

    public async Task<bool> Delete(Guid id)
    {
        Course? course = await context.Courses.SingleOrDefaultAsync(x => x.Id == id);
        if (course == null) return false;
        context.Courses.Remove(course);
        await context.SaveChangesAsync();
        return true;
    }
}