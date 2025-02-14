using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1;

public class ExamRepository(DataContext context)
{

    public async Task<List<Exam>> GetAllExamsAsync()
    {
        return await context.Exams.Include(e => e.Questions).ToListAsync();
    }

    public async Task<List<Exam>> GetUpcomingExamsAsync()
    {
        return await context.Exams
            .Where(e => e.StartDate > DateTime.UtcNow)
            .OrderBy(e => e.StartDate)
            .ToListAsync();
    }

    public async Task<List<Exam>> GetAvailableExamsAsync()
    {
        DateTime now = DateTime.UtcNow;
        return await context.Exams
            .Where(e => e.StartDate <= now && e.EndDate >= now)
            .ToListAsync();
    }
}
