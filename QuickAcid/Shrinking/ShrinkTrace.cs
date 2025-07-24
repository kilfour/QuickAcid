namespace QuickAcid.Shrinking;

public record ShrinkTrace
{
    public required int ExecutionId { get; init; }
    public required string Key { get; init; }
    public required string Name { get; init; }
    public required ShrinkIntent Intent { get; init; }
    public required object? Original { get; init; }
    public required object? Result { get; init; }
    public string Strategy { get; init; } = "";

    public bool IsKeep => Intent == ShrinkIntent.Keep;
    public bool IsReplacement => Intent == ShrinkIntent.Replace;
    public bool IsRemoved => Intent == ShrinkIntent.Remove;
    public bool IsIrrelevant => Intent == ShrinkIntent.Irrelevant;
}


