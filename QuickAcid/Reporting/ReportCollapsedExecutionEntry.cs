using System.Text;

namespace QuickAcid.Reporting;

public class ReportCollapsedExecutionEntry : ReportEntry
{
    public int Times { get; }

    public ReportCollapsedExecutionEntry(string key, int times) : base(key)
    {
        Times = times;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        string text;
        if (string.IsNullOrEmpty(Key))
        {
            text = $"  EXECUTIONS: {Times} trivial runs (no observable input/state)";
        }
        else
        {
            text = $"  EXECUTE : {Key} ({Times} Times)";
        }
        var line = " " + new string('─', 50);
        stringBuilder.AppendLine(line);
        stringBuilder.Append(text);
        return stringBuilder.ToString();
    }
}
