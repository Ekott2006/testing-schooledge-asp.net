using Bogus;
using Domain.Model;
using Domain.Model.Helpers;

namespace Domain.Seed;

public sealed class InstitutionFaker : Faker<Institution>
{
    public InstitutionFaker(Random random,List<string> academicSessions)
    {
        InstitutionType[] institutionTypes = Enum.GetValues<InstitutionType>();
        RuleFor(c => c.Name, f => f.Lorem.Sentence());
        RuleFor(c => c.Code, f => f.Lorem.Word());
        RuleFor(c => c.CurrentAcademicSession, _ => academicSessions.Last());
        RuleFor(c => c.LogoUrl, _ => "https://avatar.iran.liara.run/public");
        RuleFor(c => c.Type, _ => institutionTypes[random.Next(institutionTypes.Length)]);
    }
}