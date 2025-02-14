using Domain.Data;
using Domain.Dto.Question;
using Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

// TODO: Work Here
public class QuestionRepository(DataContext context)
{
    public async Task<bool> Upsert(QuestionRequest request)
    {
        Exam? exam = await context.Exams.FirstOrDefaultAsync(x => x.Id == request.ExamId);
        Question? existingEntity = await context.Questions
            .FirstOrDefaultAsync(x => x.QuestionNumber == request.QuestionNumber);

        if (exam == null) return false;
        if (!request.Choices.Contains(request.CorrectAnswer)) return false;
        // TODO: Ensure this logic is correct
        if (request.QuestionNumber > exam.TotalQuestions) return false;

        if (existingEntity == null)
        {
            await context.Questions.AddAsync(request); // Insert new record
        }
        else
        {
            context.Entry(existingEntity).CurrentValues.SetValues(request); // Update record
        }

        await context.SaveChangesAsync();
        return true;
    }

    // TODO: Optimize
    public async Task BulkUpsert(List<QuestionRequest> requests, CancellationToken cancellationToken)
    {
        List<Guid> examIds = requests.Select(r => r.ExamId).Distinct().ToList();
        List<int> questionNumbers = requests.Select(r => r.QuestionNumber).Distinct().ToList();

        // Fetch all necessary exams at once
        Dictionary<Guid, Exam> exams = await context.Exams
            .Where(e => examIds.Contains(e.Id))
            .ToDictionaryAsync(e => e.Id, cancellationToken: cancellationToken);

        // Fetch existing questions at once
        Dictionary<(Guid ExamId, int QuestionNumber), Question> existingQuestions = await context.Questions
            .Where(q => examIds.Contains(q.ExamId) && questionNumbers.Contains(q.QuestionNumber))
            .ToDictionaryAsync(q => (q.ExamId, q.QuestionNumber), cancellationToken: cancellationToken);

        List<Question> newQuestions = [];
        List<Question> updatedQuestions = [];

        foreach (QuestionRequest request in requests.TakeWhile(_ => !cancellationToken.IsCancellationRequested))
        {
            // Validate Exam existence
            if (!exams.TryGetValue(request.ExamId, out Exam? exam)) continue;

            // Validate answer choice
            if (!request.Choices.Contains(request.CorrectAnswer)) continue;

            // Validate question number constraint
            if (request.QuestionNumber > exam.TotalQuestions) continue;

            (Guid ExamId, int QuestionNumber) key = (request.ExamId, request.QuestionNumber);

            if (existingQuestions.TryGetValue(key, out Question? existingEntity))
            {
                // Update existing question
                context.Entry(existingEntity).CurrentValues.SetValues(request);
                updatedQuestions.Add(existingEntity);
            }
            else
            {
                // Create new question
                newQuestions.Add(request); // Assuming a mapping method
            }
        }

        // Bulk insert new questions
        if (newQuestions.Count != 0) await context.Questions.AddRangeAsync(newQuestions, cancellationToken);

        // Bulk update existing questions
        if (updatedQuestions.Count != 0) context.Questions.UpdateRange(updatedQuestions);

        await context.SaveChangesAsync(cancellationToken);
    }


    public async Task<bool> Delete(Guid id)
    {
        Faculty? faculty = await context.Faculties.SingleOrDefaultAsync(x => x.Id == id);
        if (faculty == null) return false;
        context.Faculties.Remove(faculty);
        await context.SaveChangesAsync();
        return true;
    }
    
    public async Task<bool> Restore(Guid id)
    {
        Faculty? faculty = await context.Faculties.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == id);
        if (faculty == null) return false;
        faculty.IsDeleted = false;
        await context.SaveChangesAsync();
        return true;
    }
}