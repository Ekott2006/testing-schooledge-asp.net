using Domain.Dto.Question;
using Domain.Dto.StudentExam;

namespace Domain.Model;

public class StudentAnswerResponse()
{
    public Guid Id { get; set; }
    public string Answer { get; set; }

    public Guid? StudentExamId { get; set; }
    public QuestionResponse? Question { get; set; }

    public StudentAnswerResponse(StudentAnswer? studentExam) : this()
    {
        if (studentExam == null) return;    
        Id = studentExam.Id;
        Answer = studentExam.Answer;
        StudentExamId = studentExam.StudentExamId;
        Question = new QuestionResponse(studentExam.Question);
    }

}