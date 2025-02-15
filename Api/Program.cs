using System.Text.Json.Serialization;
using Api;
using Api.BackgroundTask;
using Api.Middleware;
using Api.Routes;
using Domain.Data;
using Domain.Model;
using Domain.Model.Helpers;
using Domain.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

const string dbName = "app.db";
const string connectionString = $"Data Source={dbName}";
// File.Delete(dbName);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddSingleton<StudentFileProcessor>();
builder.Services.AddHostedService<StudentFileProcessor>();
builder.Services.AddSingleton<QuestionFileProcessor>();
builder.Services.AddHostedService<QuestionFileProcessor>();

builder.Services.AddScoped<AdminRepository>();
builder.Services.AddScoped<AuthManager>();
builder.Services.AddScoped<CourseRepository>();
builder.Services.AddScoped<DepartmentRepository>();
builder.Services.AddScoped<ExamRepository>();
builder.Services.AddScoped<FacultyRepository>();
builder.Services.AddScoped<InstitutionRepository>();
builder.Services.AddScoped<QuestionRepository>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<StudentExamRepository>();
builder.Services.AddScoped<UserRepository>();
// builder.Services.AddScoped<Repository>();
builder.Services.AddDbContext<DataContext>(options => options.UseSqlite(connectionString));

builder.Services.AddAuthorization();
builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789/-._@+";
    })
    .AddDefaultTokenProviders()
    .AddEntityFrameworkStores<DataContext>();
builder.AddCustomJwtMiddleware();

builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddCustomOpenApiMiddleware();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddAntiforgery();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "OpenAPI V1"));
    app.UseReDoc(c =>
    {
        c.DocumentTitle = "REDOC API Documentation";
        c.SpecUrl = "/openapi/v1.json";
    });   

    // TODO: Remove this and use migrations
    app.Services.UseSeedDatabaseMiddleware();
}

app.UseHttpsRedirection();
app.MapStaticAssets();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/", () => new { Message = "Hello World" });

RouteGroupBuilder accountGroup = app.MapGroup("account");
accountGroup.MapPost("student/login", StudentRoutes.Login);
accountGroup.MapGet("student/profile", StudentRoutes.GetProfile).CustomAuthorization(UserRole.Student);

accountGroup.MapPost("admin/login", AdminRoutes.Login);
accountGroup.MapGet("admin/profile", AdminRoutes.GetProfile).CustomAuthorization(UserRole.Admin);

accountGroup.MapPost("refresh", UserRoutes.Refresh);
accountGroup.MapPost("logout", UserRoutes.Logout);
accountGroup.MapPost("changePassword", UserRoutes.ChangePassword);
accountGroup.MapPost("profile-image", UserRoutes.SetProfileImage).DisableAntiforgery();

RouteGroupBuilder institutionGroup = app.MapGroup("institutions");
institutionGroup.MapGet("/", InstitutionRoutes.Get);
institutionGroup.MapPost("/", InstitutionRoutes.Create);
institutionGroup.MapPut("/{id:guid}", InstitutionRoutes.Update);
institutionGroup.MapPost("profile-image", InstitutionRoutes.SetProfileImage).DisableAntiforgery();

RouteGroupBuilder facultyGroup = app.MapGroup("faculties");
facultyGroup.MapGet("/", FacultyRoutes.Get);
facultyGroup.MapPost("/", FacultyRoutes.Create);
facultyGroup.MapPut("/{id:guid}", FacultyRoutes.Update);
facultyGroup.MapPatch("/restore/{id:guid}", FacultyRoutes.Restore);
facultyGroup.MapDelete("/{id:guid}", FacultyRoutes.Delete);

RouteGroupBuilder courseGroup = app.MapGroup("courses");
courseGroup.MapGet("/", CourseRoutes.Get);
courseGroup.MapPost("/", CourseRoutes.Create);
courseGroup.MapPut("/{id:guid}", CourseRoutes.Update);
courseGroup.MapPatch("/restore/{id:guid}", CourseRoutes.Restore);
courseGroup.MapDelete("/{id:guid}", CourseRoutes.Delete);

RouteGroupBuilder departmentGroup = app.MapGroup("departments");
departmentGroup.MapGet("/", DepartmentRoutes.Get);
departmentGroup.MapPost("/", DepartmentRoutes.Create);
departmentGroup.MapPut("/{id:guid}", DepartmentRoutes.Update);
departmentGroup.MapPatch("/restore/{id:guid}", DepartmentRoutes.Restore);
departmentGroup.MapDelete("/{id:guid}", DepartmentRoutes.Delete);

RouteGroupBuilder studentGroup = app.MapGroup("students");
studentGroup.MapGet("/", StudentRoutes.GetAll);
studentGroup.MapGet("/{id}", StudentRoutes.Get);
studentGroup.MapPost("/", StudentRoutes.Create);
studentGroup.MapPost("/bulk", StudentRoutes.BulkCreate).DisableAntiforgery();;
studentGroup.MapPut("/{id:guid}", StudentRoutes.Update);
studentGroup.MapDelete("/{id:guid}", StudentRoutes.Delete);
studentGroup.MapGet("exams/upcoming", StudentRoutes.GetUpcomingExams).CustomAuthorization(UserRole.Student);
studentGroup.MapGet("exams/available", StudentRoutes.GetAvailableExams).CustomAuthorization(UserRole.Student);

RouteGroupBuilder studentExamGroup = studentGroup.MapGroup("exams"); // /students/exams
studentExamGroup.MapGet("/", StudentExamRoutes.GetAll);
studentExamGroup.MapGet("/{id:guid}", StudentExamRoutes.Get);
studentExamGroup.MapGet("/results/{id:guid}", StudentExamRoutes.GetResults);
studentExamGroup.MapPatch("/start/{examId:guid}", StudentExamRoutes.Start);
studentExamGroup.MapPatch("/submit/{id:guid}", StudentExamRoutes.Submit);
studentExamGroup.MapPatch("/submit/answer", StudentExamRoutes.SubmitAnswer);

RouteGroupBuilder adminGroup = app.MapGroup("admins");
adminGroup.MapGet("/", AdminRoutes.GetAll);
adminGroup.MapGet("/{id:guid}", AdminRoutes.Get);
adminGroup.MapPost("/", AdminRoutes.Create);
adminGroup.MapPut("/{id:guid}", AdminRoutes.Update);
adminGroup.MapDelete("/{id:guid}", AdminRoutes.Delete);

RouteGroupBuilder examGroup = app.MapGroup("exams");
examGroup.MapGet("/", ExamRoutes.GetAll);
examGroup.MapGet("/{id:guid}", ExamRoutes.Get);
examGroup.MapPost("/", ExamRoutes.Create);
examGroup.MapPut("/{id:guid}", ExamRoutes.Update);
examGroup.MapPatch("/{id:guid}", ExamRoutes.Restore);
examGroup.MapDelete("/{id:guid}", ExamRoutes.Delete);

RouteGroupBuilder questionGroup = app.MapGroup("questions");
questionGroup.MapPost("/upsert", QuestionRoutes.Upsert);
questionGroup.MapPost("/upsert/bulk", QuestionRoutes.BulkUpsert).DisableAntiforgery();
questionGroup.MapPatch("/{id:guid}", QuestionRoutes.Restore);
questionGroup.MapDelete("/{id:guid}", QuestionRoutes.Delete);

app.Run();