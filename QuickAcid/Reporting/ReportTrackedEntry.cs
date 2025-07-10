namespace QuickAcid.Reporting;

public class ReportTrackedEntry : ReportEntry
{
    public object? Value { get; set; }

    public ReportTrackedEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        return $"   => {Key} (tracked) : {Value}";
    }
}
