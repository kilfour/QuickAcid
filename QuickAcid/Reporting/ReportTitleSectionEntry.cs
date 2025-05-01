using System.Text;

namespace QuickAcid.Reporting;

public class ReportTitleSectionEntry : IAmAReportEntry
{
    public List<string> Title;
    public ReportTitleSectionEntry(List<string> title) { Title = title; }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(" ----------------------------------------");
        foreach (var str in Title)
            stringBuilder.AppendLine($" -- {str}");
        stringBuilder.Append(" ----------------------------------------");
        return stringBuilder.ToString();
    }
}
