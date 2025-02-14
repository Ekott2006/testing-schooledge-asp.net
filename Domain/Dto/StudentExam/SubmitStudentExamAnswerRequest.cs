namespace Domain.Dto.StudentExam;

public class SubmitStudentExamAnswerRequest
{
    public Guid StudentExamId { get; set; }
    public Guid QuestionId { get; set; }
    public string Answer { get; set; }
}