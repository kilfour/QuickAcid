namespace QuickAcid.Lab.HorsesForCourses.Abstractions;

public abstract record ComparableValue<T, TInner> : IComparable<T>
    where T : ComparableValue<T, TInner>
    where TInner : IComparable<TInner>
{
    protected abstract TInner InnerValue { get; }

    public int CompareTo(T? other)
    {
        if (other is null) return 1;
        return InnerValue.CompareTo(other.InnerValue);
    }

    public static bool operator <(ComparableValue<T, TInner> left, ComparableValue<T, TInner> right)
        => left.CompareTo((T)right) < 0;

    public static bool operator >(ComparableValue<T, TInner> left, ComparableValue<T, TInner> right)
        => left.CompareTo((T)right) > 0;

    public static bool operator <=(ComparableValue<T, TInner> left, ComparableValue<T, TInner> right)
        => left.CompareTo((T)right) <= 0;

    public static bool operator >=(ComparableValue<T, TInner> left, ComparableValue<T, TInner> right)
        => left.CompareTo((T)right) >= 0;
}