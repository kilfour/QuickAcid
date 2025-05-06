namespace QuickAcid.Reporting;

public class ReportAlwaysReportedInputEntry : ReportEntry
{
    public object? Value { get; set; }

    public ReportAlwaysReportedInputEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        return $"   => {Key} (tracked) : {Value}";
    }
}
