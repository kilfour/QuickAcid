using System.Text;

namespace QuickAcid.Reporting;

public class ReportExecutionEntry : ReportEntry
{
    public ReportExecutionEntry(string key) : base(key) { }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(" ---------------------------");
        stringBuilder.Append($" EXECUTE : {Key}");
        return stringBuilder.ToString();
    }
}
