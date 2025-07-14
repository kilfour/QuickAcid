using System.Text;

namespace QuickAcid.Reporting;

public class ReportExecutionEntry : ReportEntry
{
    public ReportExecutionEntry(string key) : base(key) { }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        string text = $"  EXECUTE : {Key}";
        //var line = " " + new string('─', text.Length);
        var line = " " + new string('─', 50);
        stringBuilder.AppendLine(line);
        stringBuilder.Append(text);
        return stringBuilder.ToString();
    }
}
