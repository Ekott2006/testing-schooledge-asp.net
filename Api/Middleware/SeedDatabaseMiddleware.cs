using System.Collections.Immutable;
using Bogus;
using Domain.Data;
using Domain.Model;
using Domain.Model.Helpers;
using Domain.Seed;
using Microsoft.AspNetCore.Identity;
using static System.Console;

namespace Api.Middleware;

public static class SeedDatabaseMiddleware
{
    public static void UseSeedDatabaseMiddleware(this IServiceProvider services)
    {
        using IServiceScope scope = services.CreateScope();
        DataContext context = scope.ServiceProvider.GetRequiredService<DataContext>();
        UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        context.Database.EnsureCreated();
        SeedDatabase(context, userManager).Wait();
        if (!context.Institutions.Any() && !context.Faculties.Any()) throw new Exception("Unable to Seed Database");
    }

    public static async Task SeedDatabase(DataContext context, UserManager<User> userManager)
    {
        Random random = new();
        Faker faker = new();
        List<string> academicSessions = ["2021/2022", "2022/2023", "2023/2024", "2024/2025"];
        List<string> adminUserNames = ["AD/2024/987"];
        List<string> studentUserNames =
        [
            "ST/2024/444",
            "ST/2024/333",
            "esut/339/030",
            "U2015/5570095",
            "esut/339/029",
            "esut/339/028",
            "esut/339/027",
            "esut/339/026",
            "esut/339/025",
        ];
        const string password = "sv+4Fn6+VK2GU5W!";

        Institution institution = new InstitutionFaker(random, academicSessions);
        await context.Institutions.AddAsync(institution);
        await context.SaveChangesAsync();

        List<Faculty> faculties = new FacultyFaker().Generate(3);
        await context.Faculties.AddRangeAsync(faculties);
        WriteLine("Faculties Count: {0}", faculties.Count);
        await context.SaveChangesAsync();

        ImmutableList<Department> departments = faculties
            .Select(faculty => new DepartmentFaker(faculty.Id).Generate(random.Next(3, 5))).SelectMany(x => x)
            .ToImmutableList();
        await context.Departments.AddRangeAsync(departments);
        WriteLine("Departments Count: {0}", departments.Count);
        await context.SaveChangesAsync();


        ImmutableList<Course> courses = departments
            .Select(department => new CourseFaker(random, department.Id).Generate(random.Next(3, 5)))
            .SelectMany(x => x).ToImmutableList();
        await context.Courses.AddRangeAsync(courses);
        WriteLine("Courses Count: {0}", courses.Count);
        await context.SaveChangesAsync();


        List<User> adminUsers = await GenerateUsers(userManager, adminUserNames, password, UserRole.Admin);
        ImmutableList<Admin> admins = adminUsers.Select(user => (Admin)new AdminFaker(user.Id)).ToImmutableList();
        await context.Admins.AddRangeAsync(admins);
        WriteLine("Admins Count: {0}", admins.Count);
        await context.SaveChangesAsync();


        List<User> studentUsers = await GenerateUsers(userManager, studentUserNames, password, UserRole.Student);
        ImmutableList<Student> students = studentUsers.Select(user =>
                (Student)new StudentFaker(random, user.Id, departments[random.Next(departments.Count)].Id))
            .ToImmutableList();
        await context.Students.AddRangeAsync(students);
        WriteLine("Students Count: {0}", students.Count);
        await context.SaveChangesAsync();


        ImmutableList<Exam> exams = admins
            .Select(admin =>
                courses.Select(course =>
                        new ExamFaker(random, academicSessions, admin.Id, course.Id).Generate(random.Next(5)))
                    .SelectMany(x => x)).SelectMany(x => x).ToImmutableList();
        await context.Exams.AddRangeAsync(exams);
        WriteLine("Exam Count: {0}", exams.Count);
        await context.SaveChangesAsync();


        ImmutableList<Question> questions = exams
            .Select(exam =>
                Enumerable.Range(0, exam.TotalQuestions)
                    .Select(index => (Question)new QuestionFaker(faker, random, exam.Id, index)))
            .SelectMany(x => x)
            .ToImmutableList();
        await context.Questions.AddRangeAsync(questions);
        WriteLine("Questions Count: {0}", questions.Count);
        await context.SaveChangesAsync();


        ImmutableList<StudentExam> studentExams = students
            .Select(student => exams.Select(exam => new StudentExamFaker(faker, student.Id, exam).Generate(5)))
            .SelectMany(x => x).SelectMany(x => x).ToImmutableList();
        await context.StudentExams.AddRangeAsync(studentExams);
        WriteLine("Student Exams Count: {0}", studentExams.Count);
        await context.SaveChangesAsync();


        ImmutableList<StudentAnswer> studentAnswers = studentExams
            .Where(studentExam => studentExam.Status == StudentExamStatus.Completed)
            .Select(studentExam => studentExam.Exam.Questions.Select(question => (StudentAnswer)
                new StudentAnswerFaker(studentExam.Id, question.Id,
                    GetRandomAnswer(random, question.Choices, question.CorrectAnswer))))
            .SelectMany(x => x)
            .ToImmutableList();
        await context.StudentAnswers.AddRangeAsync(studentAnswers);
        WriteLine("Student Answers Count: {0}", studentAnswers.Count);
        await context.SaveChangesAsync();
    }

    private static async Task<List<User>> GenerateUsers(UserManager<User> userManager, List<string> userNames,
        string password,
        UserRole role)
    {
        List<User> users = userNames.Select(x => (User)new UserFaker(x)).ToList();
        foreach (User user in users)
        {
            IdentityResult result = await userManager.CreateAsync(user, password);
            if (result != IdentityResult.Success) throw new Exception(string.Join(", ", result.Errors));
            await userManager.AddToRolesAsync(user, [role.ToString()]);
        }

        return users;
    }

    private static string GetRandomAnswer(Random random, ICollection<string> choices, string correctAnswer)
    {
        bool returnCorrectAnswer = random.Next(2) == 0; // 50% chance for correct or wrong answer
        if (returnCorrectAnswer) return correctAnswer; // Return the correct answer

        // Get a random wrong answer from the choices
        List<string> wrongChoices = choices.Where(choice => choice != correctAnswer).ToList();
        return wrongChoices[random.Next(wrongChoices.Count)];
    }
}