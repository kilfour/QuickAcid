namespace QuickAcid.Proceedings;

public record TraceDeposition
{
    public string Label { get; }
    public string Value { get; }

    public TraceDeposition(string label, string value)
    {
        Label = label;
        Value = value;
    }
};