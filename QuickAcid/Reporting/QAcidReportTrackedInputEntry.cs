namespace QuickAcid.Reporting;

public class QAcidReportTrackedInputEntry : QAcidReportEntry
{
    public object? Value { get; set; }

    public QAcidReportTrackedInputEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        return $"{Key} (tracked) : {Value}";
    }
}
