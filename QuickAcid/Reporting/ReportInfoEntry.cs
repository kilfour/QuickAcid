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
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        stringBuilder.Append(text);
        return stringBuilder.ToString();
    }
}
