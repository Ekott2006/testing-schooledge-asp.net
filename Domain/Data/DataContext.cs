using System.Linq.Expressions;
using Domain.Model;
using Domain.Model.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Domain.Data;

public class DataContext(DbContextOptions<DataContext> options) : IdentityDbContext<User>(options)
{
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Student> Students { get; set; }

    public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    public DbSet<Course> Courses { get; set; }

    public DbSet<Department> Departments { get; set; }

    public DbSet<Exam> Exams { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Institution> Institutions { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<StudentExam> StudentExams { get; set; }
    // public DbSet<Exam> Exams { get; set; }
    public DbSet<StudentAnswer> StudentAnswers { get; set; }

    public override int SaveChanges()
    {
        HandleSoftDelete();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        HandleSoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // TODO: Seeding Identity Roles
        modelBuilder.Entity<IdentityRole>().HasData(Enum.GetValues<UserRole>()
            .Select(role => new IdentityRole { Name = role.ToString(), NormalizedName = role.ToString().ToUpper()}));

        // Apply global query filter for soft deletable entities
        foreach (IMutableEntityType entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .HasQueryFilter(GetSoftDeleteFilter(entityType.ClrType));
            }
        }
    }

    private static LambdaExpression GetSoftDeleteFilter(Type entityType)
    {
        ParameterExpression parameter = Expression.Parameter(entityType, "e");
        MemberExpression property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
        BinaryExpression condition = Expression.Equal(property, Expression.Constant(false));
        return Expression.Lambda(condition, parameter);
    }

    private void HandleSoftDelete()
    {
        foreach (EntityEntry<ISoftDeletable> entry in ChangeTracker.Entries<ISoftDeletable>())
        {
            if (entry.State != EntityState.Deleted) continue;
            entry.State = EntityState.Modified;
            entry.Entity.IsDeleted = true;
        }
    }

    private void HandleDateTimeHelper()
    {
        IEnumerable<EntityEntry> entries = ChangeTracker
            .Entries()
            .Where(e => e is { Entity: DateTimeHelper, State: EntityState.Added or EntityState.Modified });

        foreach (EntityEntry entityEntry in entries)
        {
            ((DateTimeHelper)entityEntry.Entity).UpdatedDate = DateTime.Now;

            if (entityEntry.State == EntityState.Added)
            {
                ((DateTimeHelper)entityEntry.Entity).CreatedDate = DateTime.Now;
            }
        }
    }
}