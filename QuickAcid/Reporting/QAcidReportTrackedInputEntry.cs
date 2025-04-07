namespace QuickAcid.Reporting;

public class QAcidReportAlwaysReportedInputEntry : QAcidReportEntry
{
    public object? Value { get; set; }

    public QAcidReportAlwaysReportedInputEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        return $"{Key} (tracked) : {Value}";
    }
}
