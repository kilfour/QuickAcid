using System.Text;

namespace QuickAcid.Reporting;

public class ReportSpecEntry : ReportEntry
{
    public ReportSpecEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        stringBuilder.AppendLine(" ********************************");
        stringBuilder.AppendLine($"  Spec Failed : {Key}");
        stringBuilder.AppendLine(" ********************************");
        return stringBuilder.ToString();
    }
}
