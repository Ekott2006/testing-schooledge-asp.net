using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1;

public class StudentExamRepository(DataContext context)
{
    // 🟢 Start Exam
    public async Task<StudentExam?> StartExamAsync(int examId, string studentId)
    {
        Exam? exam = await context.Exams.FindAsync(examId);
        if (exam == null || DateTime.UtcNow < exam.StartDate || DateTime.UtcNow > exam.EndDate)
            return null; // Exam not available

        StudentExam? studentExam = await context.StudentExams
            .FirstOrDefaultAsync(se => se.ExamId == examId && se.StudentId == studentId);
        // Exam already submitted
        if (studentExam?.Status == StudentExamStatus.Completed)
        {
            return null;
        }

        switch (studentExam)
        {
            case null:
                studentExam = new StudentExam
                {
                    ExamId = examId,
                    StudentId = studentId,
                    StartTime = DateTime.UtcNow,
                    EndTime = DateTime.UtcNow + exam.Duration,
                    Status = StudentExamStatus.InProgress
                };
                context.StudentExams.Add(studentExam);
                break;
            default:
                studentExam.StartTime = DateTime.UtcNow;
                studentExam.EndTime = DateTime.UtcNow + exam.Duration;
                studentExam.Status = StudentExamStatus.InProgress;
                break;
        }

        await context.SaveChangesAsync();
        return studentExam;
    }

    // 🟢 Submit Answer (Ensuring it's within exam duration)
    public async Task<bool> SubmitAnswerAsync(int studentExamId, int questionId, string answer)
    {
        StudentExam? studentExam = await context.StudentExams
            .Include(se => se.Exam)
            .Include(se => se.StudentAnswers)
            .FirstOrDefaultAsync(se => se.Id == studentExamId);

        if (studentExam is not { Status: StudentExamStatus.InProgress })
            return false;

        // Ensure answer is within the allowed duration
        if (DateTime.UtcNow > studentExam.EndTime)
        {
            studentExam.Status = StudentExamStatus.Completed;
            await context.SaveChangesAsync();
            return false; // Submission rejected
        }

        StudentAnswer? studentAnswer = studentExam.StudentAnswers.FirstOrDefault(sa => sa.QuestionId == questionId);
        if (studentAnswer == null)
        {
            studentAnswer = new StudentAnswer
            {
                StudentExamId = studentExamId,
                QuestionId = questionId,
                Answer = answer
            };
            context.StudentAnswers.Add(studentAnswer);
        }
        else
        {
            studentAnswer.Answer = answer; // Update answer
        }

        await context.SaveChangesAsync();
        return true;
    }

    // 🟢 Get Student Exams
    public async Task<List<StudentExam>> GetStudentExamsAsync(string studentId)
    {
        return await context.StudentExams
            .Where(se => se.StudentId == studentId)
            .Include(se => se.Exam)
            .ToListAsync();
    }

    // 🟢 Get Student Exam Details
    public async Task<StudentExam?> GetStudentExamByIdAsync(int studentExamId)
    {
        return await context.StudentExams
            .Include(se => se.Exam)
            .Include(se => se.StudentAnswers)
            .ThenInclude(sa => sa.Question)
            .FirstOrDefaultAsync(se => se.Id == studentExamId);
    }

    public async Task<bool> SubmitExamAsync(int studentExamId)
    {
        StudentExam? studentExam = await context.StudentExams
            .Include(se => se.Exam)
            .Include(se => se.StudentAnswers)
            .ThenInclude(sa => sa.Question)
            .FirstOrDefaultAsync(se => se.Id == studentExamId);

        if (studentExam is not { Status: StudentExamStatus.InProgress })
            return false;

        // Ensure exam duration hasn't expired

        // ✅ Auto-Grading Logic
        int totalScore = 0;
        int correct = 0;

        foreach (StudentAnswer _ in studentExam.StudentAnswers.Where(answer => answer.Answer.Trim().Equals(answer.Question.CorrectAnswer.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            correct++;
            totalScore += 1; // Assume each question is 1 mark
        }

        studentExam.TotalScore = totalScore;
        studentExam.CorrectAnswers = correct;
        studentExam.Status = StudentExamStatus.Completed;

        await context.SaveChangesAsync();
        return true;
    }

// ✅ Get Student Exam Results
    public async Task<ExamResultsResponse?> GetExamResultsAsync(int studentExamId)
    {
        StudentExam? studentExam = await context.StudentExams
            .Include(se => se.Exam).ThenInclude(exam => exam.Questions)
            .Where(x => DateTime.UtcNow  >= x.Exam.ReleaseDate && x.Status == StudentExamStatus.Completed)
            .FirstOrDefaultAsync(se => se.Id == studentExamId);

        if (studentExam == null)
            return null;

        return new ExamResultsResponse()
        {
            StudentExamId = studentExam.Id,
            ExamTitle = studentExam.Exam.Title,
            StudentId = studentExam.StudentId,
            TotalScore = studentExam.TotalScore,
            CorrectAnswers = studentExam.CorrectAnswers,
            Percentage = studentExam.Exam.Questions.Count > 0
                ? (studentExam.TotalScore * 100 / studentExam.Exam.Questions.Count)
                : 0
        };
    }
}