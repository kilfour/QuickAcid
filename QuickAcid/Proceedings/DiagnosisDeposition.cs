namespace QuickAcid.Proceedings;

public record DiagnosisDeposition
{
    public string Label { get; }
    public List<string> Traces { get; }

    public DiagnosisDeposition(string label, List<string> Traces)
    {
        Label = label;
        this.Traces = Traces;
    }
};