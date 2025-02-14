using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Exam> Exams { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<StudentExam> StudentExams { get; set; }
    public DbSet<StudentAnswer> StudentAnswers { get; set; }
}