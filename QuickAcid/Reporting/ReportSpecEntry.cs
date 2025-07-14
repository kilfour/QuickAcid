using System.Text;

namespace QuickAcid.Reporting;

public class ReportSpecEntry : ReportEntry
{
    public ReportSpecEntry(string key)
        : base(key) { }

    public override string ToString()
    {
        var text = $"  ❌ Spec Failed : {Key}";
        var line = " " + new string('═', text.Length);
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(line);
        stringBuilder.AppendLine(text);
        stringBuilder.Append(line);
        return stringBuilder.ToString();
    }
}
