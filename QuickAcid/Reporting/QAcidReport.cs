using System.Text;

namespace QuickAcid.Reporting;

public class QAcidReport
{
    private readonly List<IAmAReportEntry> entries = [];

    public List<IAmAReportEntry> Entries { get { return entries; } }

    public virtual void AddEntry(IAmAReportEntry reportEntry)
    {
        entries.Add(reportEntry);
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(" ---------------------------");
        foreach (var entry in entries)
        {
            stringBuilder.AppendLine(entry.ToString());
        }
        return stringBuilder.ToString();
    }
}
