namespace QuickAcid.Proceedings;

public record TrackedDeposition
{
    public string Label { get; }
    public string Value { get; }

    public TrackedDeposition(string label, string value)
    {
        Label = label;
        Value = value;
    }
};