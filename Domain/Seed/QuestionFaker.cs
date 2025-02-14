using Bogus;
using Domain.Model;

namespace Domain.Seed;

public sealed class QuestionFaker : Faker<Question>
{
    public QuestionFaker(Faker faker, Random random,Guid examId, int questionNumber)
    {
        int listSize = random.Next(4, 7); // Random number between 4 and 6
        List<string> choices = [];
        for (int i = 0; i < listSize; i++)
        {
            choices.Add(faker.Lorem.Word()); // Generate random words
        }
        
        RuleFor(c => c.ExamId, _ => examId);
        RuleFor(c => c.QuestionNumber, _ => questionNumber);
        RuleFor(c => c.Content, f => f.Lorem.Sentence());
        RuleFor(c => c.Choices, _ => choices);
        RuleFor(c => c.CorrectAnswer, _ => choices[random.Next(choices.Count)]);
        RuleFor(c => c.Explanation, f => f.Lorem.Sentence());
    }
}