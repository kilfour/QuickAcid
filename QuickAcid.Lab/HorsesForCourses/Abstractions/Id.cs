namespace QuickAcid.Lab.HorsesForCourses.Abstractions;

public readonly record struct Id<T>(int Value)
{
    public static Id<T> Empty => new(0);
    public static Id<T> From(int value) => new(value);
}

