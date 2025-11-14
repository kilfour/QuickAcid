using QuickAcid.Lab.HorsesForCourses.Domain.Courses.InvalidationReasons;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Courses.TimeSlots;

public record TimeSlot
{
    public CourseDay Day { get; }
    public OfficeHour Start { get; } = OfficeHour.Empty;
    public OfficeHour End { get; } = OfficeHour.Empty;

    private TimeSlot() { /*** EFC Was Here ****/ }
    private TimeSlot(CourseDay day, OfficeHour start, OfficeHour end)
    {
        Day = day;
        Start = start;
        End = end;
    }

    public static TimeSlot From(CourseDay day, int start, int end)
    {
        if (start >= end)
            throw new TimeSlotMustBeAtleastOneHourLong();
        return new TimeSlot(day, OfficeHour.From(start), OfficeHour.From(end));
    }

    public static IEnumerable<TimeSlot> EnumerableFrom<T>(
        IEnumerable<T> enumerable,
        Func<T, (CourseDay Day, int Start, int End)> getTimeSlot)
    {
        foreach (var item in enumerable)
        {
            var (day, start, end) = getTimeSlot(item);
            yield return From(day, start, end);
        }
    }

    public bool OverlapsWith(TimeSlot otherTimeSlot)
    {
        if (Day != otherTimeSlot.Day) return false;
        if (Start < otherTimeSlot.End && End > otherTimeSlot.Start) return true;
        return false;
    }

    public static bool HasOverlap(IEnumerable<TimeSlot> timeSlots)
    {
        var index = 1;
        foreach (var timeSlot in timeSlots)
        {
            foreach (var otherTimeSlot in timeSlots.Skip(index))
            {
                if (timeSlot.OverlapsWith(otherTimeSlot))
                    return true;
            }
            index++;
        }
        return false;
    }
}
