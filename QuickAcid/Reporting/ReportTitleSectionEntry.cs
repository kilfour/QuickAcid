using System.Text;

namespace QuickAcid.Reporting;

public class ReportTitleSectionEntry : IAmAReportEntry
{
    private List<string> title;
    public ReportTitleSectionEntry(List<string> title) { this.title = title; }


    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(" ----------------------------------------");
        foreach (var str in title)
            stringBuilder.AppendLine($" -- {str}");
        stringBuilder.AppendLine(" ----------------------------------------");
        return stringBuilder.ToString();
    }
}
