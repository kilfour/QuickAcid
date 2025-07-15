using QuickAcid.Reporting;
using QuickPulse;

namespace QuickAcid.Bolts;

public class ReportArtery : IArtery
{
    private readonly Report report;

    public ReportArtery(Report report)
    {
        this.report = report;
    }
    public void Flow(params object[] data)
    {
        foreach (var item in data)
        {
            report.AddEntry(new ReportInfoEntry((string)item));
        }
    }
}
