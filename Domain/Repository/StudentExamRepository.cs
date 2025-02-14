﻿using Domain.Data;
using Domain.Dto.StudentExam;
using Domain.Model;
using Domain.Model.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Domain.Repository;

public class StudentExamRepository(DataContext context)
{
    // 🟢 Start Exam
    public async Task<StudentExam?> StartExam(string userId, Guid examId)
    {
        Exam? exam = await context.Exams.FirstOrDefaultAsync(x => x.Id == examId);
        Student? student = await context.Students.FirstOrDefaultAsync(x => x.UserId == userId);
        if (student == null || exam == null || DateTime.UtcNow < exam.StartDateTime ||
            DateTime.UtcNow > exam.ReleaseDate)
            return null; // Exam not available

        StudentExam? studentExam = await context.StudentExams
            .FirstOrDefaultAsync(se => se.ExamId == examId && se.StudentId == student.Id);
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
                    StudentId = student.Id,
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
    public async Task<bool> SubmitAnswer(string userId, SubmitStudentExamAnswerRequest request)
    {
        StudentExam? studentExam = await context.StudentExams
            .Include(se => se.Exam)
            .Include(se => se.StudentAnswers)
            .Where(x => x.Student.UserId == userId)
            .FirstOrDefaultAsync(se => se.Id == request.StudentExamId);

        if (studentExam is not { Status: StudentExamStatus.InProgress })
            return false;

        // Ensure answer is within the allowed duration
        if (DateTime.UtcNow > studentExam.EndTime)
        {
            studentExam.Status = StudentExamStatus.Completed;
            await context.SaveChangesAsync();
            return false; // Submission rejected
        }

        StudentAnswer? studentAnswer = studentExam.StudentAnswers.FirstOrDefault(sa => sa.QuestionId == request.QuestionId);
        if (studentAnswer == null)
        {
            studentAnswer = new StudentAnswer
            {
                StudentExamId = request.StudentExamId,
                QuestionId = request.QuestionId,
                Answer = request.Answer
            };
            context.StudentAnswers.Add(studentAnswer);
        }
        else
        {
            studentAnswer.Answer = request.Answer; // Update answer
        }

        await context.SaveChangesAsync();
        return true;
    }

    // 🟢 Get Student Exams
    public async Task<List<StudentExam>> GetStudentExams(string userId)
    {
        return await context.StudentExams
            .Where(se => se.Student.UserId == userId)
            .Include(se => se.Exam)
            .ToListAsync();
    }

    // 🟢 Get Student Exam Details
    public async Task<StudentExam?> GetStudentExamById(string userId, Guid studentExamId)
    {
        return await context.StudentExams
            .Include(se => se.Exam)
            .Include(se => se.StudentAnswers)
            .ThenInclude(sa => sa.Question)
            .Where(se => se.Student.UserId == userId)
            .FirstOrDefaultAsync(se => se.Id == studentExamId);
    }

    public async Task<bool> SubmitExam(string userId, Guid studentExamId)
    {
        StudentExam? studentExam = await context.StudentExams
            .Include(se => se.Exam)
            .Include(se => se.StudentAnswers)
            .ThenInclude(sa => sa.Question)
            .Where(se => se.Student.UserId == userId)
            .FirstOrDefaultAsync(se => se.Id == studentExamId);

        if (studentExam is not { Status: StudentExamStatus.InProgress })
            return false;

        // Ensure exam duration hasn't expired

        // ✅ Auto-Grading Logic
        int totalScore = 0;
        int correct = 0;

        foreach (StudentAnswer _ in studentExam.StudentAnswers.Where(answer =>
                     answer.Answer.Trim().Equals(answer.Question.CorrectAnswer.Trim(),
                         StringComparison.OrdinalIgnoreCase)))
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
    public async Task<StudentExamResultResponse?> GetExamResults(string userId, Guid studentExamId)
    {
        return  await context.StudentExams
            .Include(se => se.Exam).ThenInclude(exam => exam.Questions)
            .Where(x => DateTime.UtcNow >= x.Exam.ReleaseDate && x.Status == StudentExamStatus.Completed && x.Student.UserId == userId)
            .Select(x => new StudentExamResultResponse(x.Id, x.CorrectAnswers, x.TotalScore))
            .FirstOrDefaultAsync(se => se.Id == studentExamId);
    }
}