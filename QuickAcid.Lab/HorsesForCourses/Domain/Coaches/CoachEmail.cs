using QuickAcid.Lab.HorsesForCourses.Abstractions;
using QuickAcid.Lab.HorsesForCourses.Domain.Coaches.InvalidationReasons;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Coaches;

public record CoachEmail : DefaultString<CoachEmailCanNotBeEmpty, CoachEmailCanNotBeTooLong>
{
    public CoachEmail(string value) : base(value) { }
    private CoachEmail() { }
    public static CoachEmail Empty => new();
}
