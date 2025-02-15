using Domain.Dto.Exam;
using Domain.Dto.Student;
using Domain.Model;
using Domain.Model.Helpers;

namespace Domain.Dto.StudentExam;

public class StudentExamResponse()
{
    public Guid Id { get; set; }
    public Guid? ExamId { get; set; }
    public Guid? StudentId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public StudentExamStatus Status { get; set; }

    // ✅ Auto-Grading Fields
    public int TotalScore { get; set; } = 0;
    public int CorrectAnswers { get; set; } = 0;

    public List<StudentAnswerResponse> StudentAnswers { get; set; } = [];

    public StudentExamResponse(Model.StudentExam? studentExam) : this()
    {
        if (studentExam == null) return;
        Id = studentExam.Id;
        ExamId = studentExam.ExamId;
        StudentId = studentExam.StudentId;
        StartTime = studentExam.StartTime;
        EndTime = studentExam.EndTime;
        Status = studentExam.Status;
        TotalScore = studentExam.TotalScore;
        CorrectAnswers = studentExam.CorrectAnswers;
        StudentAnswers = studentExam.StudentAnswers.Select(x => new StudentAnswerResponse(x)).ToList();
    }
}