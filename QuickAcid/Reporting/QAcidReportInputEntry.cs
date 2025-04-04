namespace QuickAcid.Reporting;

public class QAcidReportInputEntry : QAcidReportEntry
{
    public object? Value { get; set; }

    public QAcidReportInputEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        if (Value == null)
            return $"Input : {Key}";
        return $"Input : {Key} = {Value}";
    }
}
