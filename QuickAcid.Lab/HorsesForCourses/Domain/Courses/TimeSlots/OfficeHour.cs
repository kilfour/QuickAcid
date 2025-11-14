using QuickAcid.Lab.HorsesForCourses.Abstractions;
using QuickAcid.Lab.HorsesForCourses.Domain.Courses.InvalidationReasons;

namespace QuickAcid.Lab.HorsesForCourses.Domain.Courses.TimeSlots;

public record OfficeHour : ComparableValue<OfficeHour, int>
{
    public int Value { get; }

    protected override int InnerValue => Value;

    private OfficeHour() { /*** EFC Was Here ****/ }

    private OfficeHour(int value)
    {
        Value = value;
    }

    public static OfficeHour Empty => new(-1); // bit of a hack

    public static OfficeHour From(int value)
    {
        CheckValue(value);
        return new OfficeHour(value);
    }

    private static void CheckValue(int value)
    {
        if (value < 9 || value > 17)
            throw new InvalidOfficeHour();
    }

    public static int operator -(OfficeHour a, OfficeHour b)
        => a.Value - b.Value;
}
