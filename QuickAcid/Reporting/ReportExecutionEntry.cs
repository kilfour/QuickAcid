using System.Text;

namespace QuickAcid.Reporting;

public class ReportExecutionEntry : ReportEntry
{
    private readonly int executionId;

    public ReportExecutionEntry(string key, int executionId) : base(key)
    {
        this.executionId = executionId;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        string text = $"  Executed ({executionId}): {Key}";
        var line = " " + new string('─', 50);
        stringBuilder.AppendLine(line);
        stringBuilder.Append(text);
        return stringBuilder.ToString();
    }
}
