using QuickAcid.Lab.HorsesForCourses.Abstractions;
using QuickAcid.Lab.HorsesForCourses.Domain.Coaches.InvalidationReasons;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Coaches;

public record CoachName : DefaultString<CoachNameCanNotBeEmpty, CoachNameCanNotBeTooLong>
{
    public CoachName(string value) : base(value) { }
    protected CoachName() { }
    public static CoachName Empty => new();
}
