namespace QuickAcid.Reporting;

public class ReportTraceEntry : ReportEntry
{
    public object? Value { get; set; }

    public ReportTraceEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        return $"   - Trace : {Key} = {Value}";
    }
}
