namespace QuickAcid.Reporting;

public abstract class QAcidReportEntry : IAmAReportEntry
{
    protected readonly string TheKey;
    public string Key => TheKey;

    protected QAcidReportEntry(string key)
    {
        TheKey = key;
    }
}
