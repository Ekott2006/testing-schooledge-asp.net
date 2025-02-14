namespace Domain.Dto.StudentExam;

public record StudentExamResultResponse(Guid Id, int CorrectAnswers, decimal TotalScore);