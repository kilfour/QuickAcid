namespace QuickAcid.Lab.HorsesForCourses.Abstractions;

public record DefaultString<TNotEmpty, TTooLong>
    where TNotEmpty : Exception, new()
    where TTooLong : Exception, new()
{
    public string Value { get; } = string.Empty;
    protected DefaultString() { }
    public DefaultString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new TNotEmpty();
        if (value.Length > DefaultString.MaxLength)
            throw new TTooLong();
        Value = value;
    }
}
public record DefaultString
{
    public const int MaxLength = 100;
}