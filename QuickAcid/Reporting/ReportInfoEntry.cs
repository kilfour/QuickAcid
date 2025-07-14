using System.Text;

namespace QuickAcid.Reporting;

public class ReportInfoEntry : IAmAReportEntry
{
    private readonly string text;

    public ReportInfoEntry(string text)
    {
        this.text = text;
    }

    public override string ToString()
    {
        return text;
    }
}
