using QuickAcid.Lab.HorsesForCourses.Abstractions;
using QuickAcid.Lab.HorsesForCourses.Domain.Courses;
using QuickAcid.Lab.HorsesForCourses.Domain.Coaches;

namespace QuickAcid.Lab.HorsesForCourses.Domain;

public class UnavailableFor(Id<Coach> CoachId, Id<Course> CourseId) : DomainEntity<UnavailableFor>
{
    public Id<Coach> CoachId { get; } = CoachId;
    public Id<Course> CourseId { get; } = CourseId;
}
