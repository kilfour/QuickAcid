using QuickAcid.Lab.HorsesForCourses.Domain.Courses;
using QuickAcid.Lab.HorsesForCourses.Domain.Coaches;

namespace QuickAcid.Lab.HorsesForCourses.Domain;

public class CoachUnavailableForCourseUpdater
{
    public static async IAsyncEnumerable<UnavailableFor> FromCoachAsync(
        Coach coach,
        IAsyncEnumerable<Course> courses)
    {
        await foreach (var course in courses.ConfigureAwait(false))
        {
            if (!course.IsConfirmed)
                continue;
            if (course.AssignedCoach != null)
                continue;
            if (coach.IsSuitableFor(course) && coach.IsAvailableFor(course))
                continue;

            yield return new UnavailableFor(coach.Id, course.Id);
        }
    }

    public static async IAsyncEnumerable<UnavailableFor> FromCourseAsync(
        Course course,
        IAsyncEnumerable<Coach> coaches)
    {
        if (!course.IsConfirmed || course.AssignedCoach != null)
            yield break;

        await foreach (var coach in coaches.ConfigureAwait(false))
        {
            if (coach.IsSuitableFor(course) && coach.IsAvailableFor(course))
                continue;

            yield return new UnavailableFor(coach.Id, course.Id);
        }
    }
}

