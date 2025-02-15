using Domain.Data;
using Domain.Dto;
using Domain.Dto.Exam;
using Domain.Dto.Student;
using Domain.Helpers;
using Domain.Model;
using Domain.Model.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class StudentRepository(DataContext context)
{
    // TODO:Convert to a service 
    public async Task Create(string userId, StudentRequest request)
    {
        ICollection<Course> courses = await context.Courses
            .Where(x => x.DepartmentId == request.DepartmentId && x.Level == request.Level).ToListAsync();
        await context.Students.AddAsync(request.ToStudent(userId, courses));
        await context.SaveChangesAsync();
    }

    // TODO:Convert to a service 
    public async Task CreateBulk(List<(string userId, StudentRequest request)> studentRequests,
        CancellationToken cancellationToken)
    {
        List<Guid> departmentIds = studentRequests.Select(r => r.request.DepartmentId).Distinct().ToList();
        List<CourseLevel> levels = studentRequests.Select(r => r.request.Level).Distinct().ToList();

        // Fetch all required courses in one query
        List<Course> courses = await context.Courses
            .Where(x => departmentIds.Contains(x.DepartmentId) && levels.Contains(x.Level))
            .ToListAsync(cancellationToken: cancellationToken);

        // Map requests to students, ensuring each userId is unique
        List<Student> students = studentRequests.Select(sr =>
            sr.request.ToStudent(sr.userId, courses
                .Where(c => c.DepartmentId == sr.request.DepartmentId && c.Level == sr.request.Level)
                .ToList())
        ).ToList();

        // Bulk insert
        await context.Students.AddRangeAsync(students, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }


    public async Task<StudentResponse?> Get(string userId)
    {
        Student? student = await context.Students.Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId);
        return new StudentResponse(student);
    }

    public async Task<PagedResult<StudentResponse>> GetAll(GetStudentRequest request)
    {
        IQueryable<Student> query = context.Students.Select(x => x).AsQueryable();
        if (!string.IsNullOrEmpty(request.Search))
            query = query.Where(e => (e.User.UserName ?? string.Empty).Contains(request.Search));
        if (request.FacultyId.HasValue)
            query = query.Where(e => e.Department.FacultyId == request.FacultyId);
        if (request.DepartmentId.HasValue)
            query = query.Where(e => e.DepartmentId == request.DepartmentId);
        if (request.Level != null)
            query = query.Where(e => e.Level == request.Level);

        return await query.Select(x => new StudentResponse(x)).ToPagedListAsync(request.PageNumber, request.PageSize);
    }

    public async Task Update(string userId, UpdateStudentRequest request)
    {
        await context.Students.Where(x => x.UserId == userId)
            .ExecuteUpdateAsync(x => x
                .SetProperty(p => p.DepartmentId, request.DepartmentId)
                .SetProperty(p => p.Level, request.Level));
    }

    public async Task Delete(string userId)
    {
        await context.Students.Where(x => x.UserId == userId).ExecuteDeleteAsync();
    }

    private IQueryable<ExamResponse> GetExamsQuery(string userId)
    {
        return context.Students
            .Where(s => s.UserId == userId)
            .SelectMany(s => s.Courses.SelectMany(c => c.Exams))
            .Where(e => e.StartDateTime > DateTime.UtcNow).Select(x => new ExamResponse(x));
    }

    public async Task<List<ExamResponse>> GetUpcomingExams(string userId)
    {
        return await GetExamsQuery(userId).OrderBy(e => e.StartDateTime)
            .ToListAsync();
    }

    public async Task<List<ExamResponse>> GetAvailableExams(string userId)
    {
        DateTime now = DateTime.UtcNow;
        return await GetExamsQuery(userId).Where(e => e.StartDateTime <= now && e.ReleaseDate >= now)
            .ToListAsync();
    }
    //
    // public async Task<IEnumerable<Exam>?> GetExams(string id, StudentGetExamOptions options)
    // {
    //     DateTime dateTime = DateTime.Now;
    //     IQueryable<Exam> query = context.StudentCourses.Include(studentCourse => studentCourse.Course)
    //         .ThenInclude(course => course.Exams)
    //         .Where(x => x.StudentId == id)
    //         .Select(x => x.Course.Exams)
    //         .SelectMany(x => x);
    //     switch (options)
    //     {
    //         case StudentGetExamOptions.Upcoming:
    //             query = query.Where(x => x.EndTime < dateTime);
    //             break;
    //         case StudentGetExamOptions.Available:
    //             query = query.Where(x => x.StartTime < dateTime && x.EndTime > dateTime);
    //             break;
    //         case StudentGetExamOptions.All:
    //             break;
    //         default:
    //             throw new ArgumentOutOfRangeException(nameof(options), options, null);
    //     }
    //
    //     return await query.ToListAsync();
    // }
}