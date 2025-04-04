namespace QuickAcid.Reporting;

public class ReportInputEntry : QAcidReportEntry
{
    public object? Value { get; set; }

    public ReportInputEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        if (Value == null)
            return $"Input : {Key}";
        return $"Input : {Key} = {Value}";
    }
}
