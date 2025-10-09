namespace QuickAcid.Proceedings;

public record StashedDeposition
{
    public string Label { get; }
    public string Value { get; }

    public StashedDeposition(string label, string value)
    {
        Label = label;
        Value = value;
    }
};