using System.Text;

namespace QuickAcid.Reporting;

public class ReportTitleSectionEntry : IAmAReportEntry
{
    private string title = "";
    public ReportTitleSectionEntry(string title) { this.title = title; }


    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(" ----------------------------------------");
        stringBuilder.AppendLine($" -- {title}");
        stringBuilder.AppendLine(" ----------------------------------------");
        stringBuilder.Append(" RUN START :");
        return stringBuilder.ToString();
    }
}