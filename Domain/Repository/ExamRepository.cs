using Domain.Data;
using Domain.Dto;
using Domain.Dto.Exam;
using Domain.Helpers;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class ExamRepository(DataContext context)
{
    public async Task<PagedResult<ExamResponse>> Get(GetExamRequest request)
    {
        IQueryable<Exam> query = context.Exams.AsQueryable();
        if (request.Status != null)
            query = query.Where(e => e.Status == request.Status);
        if (!string.IsNullOrEmpty(request.Title))
            query = query.Where(e => e.Title.Contains(request.Title));
        if (request.FacultyId.HasValue)
            query = query.Where(e => e.Course.Department.FacultyId == request.FacultyId);
        if (request.DepartmentId.HasValue)
            query = query.Where(e => e.Course.DepartmentId == request.DepartmentId);
        if (request.Level != null)
            query = query.Where(e => e.Course.Level == request.Level);

        return await query.Select(x => new ExamResponse(x)).ToPagedListAsync(request.PageNumber, request.PageSize);
    }

    public async Task<ExamResponse?> Get(Guid id)
    {
        Exam? exam = await context.Exams
            .Include(exam => exam.Course)
            .Include(x => x.Questions)
            .FirstOrDefaultAsync(x => x.Id == id);
        return new ExamResponse(exam);
    }

    public async Task<bool> Create(string userId, ExamRequest request)
    {
        var result = await context.Students.Select(x => new {x.UserId, x.Id
    }).FirstOrDefaultAsync(s => s.UserId == userId);
        if (result == null) return false;
        await context.Exams.AddAsync(request.ToExam(result.Id));
        await context.SaveChangesAsync();
        return true;
    }

    public async Task Update(Guid id, ExamRequest request)
    {
        await context.Exams.Where(x => x.Id == id)
            .ExecuteUpdateAsync(x => x
                .SetProperty(exam => exam.CourseId, request.CourseId)
                .SetProperty(exam => exam.Title, request.Title)
                .SetProperty(exam => exam.Instructions, request.Instructions)
                .SetProperty(exam => exam.Instructor, request.Instructor)
                .SetProperty(exam => exam.StartDateTime, request.StartDateTime)
                .SetProperty(exam => exam.Duration, request.Duration)
                .SetProperty(exam => exam.MaxAttempts, request.MaxAttempts)
                .SetProperty(exam => exam.PointsPerQuestion, request.PointsPerQuestion)
                .SetProperty(exam => exam.TotalQuestions, request.TotalQuestions)
                .SetProperty(exam => exam.AcademicSession, request.AcademicSession)
            );
    }


    public async Task<bool> Restore(Guid id)
    {
        Exam? exam = await context.Exams.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id);
        if (exam == null) return false;

        exam.IsDeleted = false;
        await context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Delete(Guid id)
    {
        Exam? exam = await context.Exams.SingleOrDefaultAsync(x => x.Id == id);
        if (exam == null) return false;

        context.Exams.Remove(exam);
        await context.SaveChangesAsync();
        return true;
    }
}