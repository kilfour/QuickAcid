namespace QuickAcid.Proceedings;

public class InputDeposition
{
    public string Label { get; }
    public object Value { get; }

    public InputDeposition(string label, object value)
    {
        Label = label;
        Value = value;
    }
};