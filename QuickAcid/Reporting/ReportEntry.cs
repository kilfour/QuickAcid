namespace QuickAcid.Reporting;

public abstract class ReportEntry : IAmAReportEntry
{
    protected readonly string TheKey;
    public string Key => TheKey;

    protected ReportEntry(string key)
    {
        TheKey = key;
    }
}
