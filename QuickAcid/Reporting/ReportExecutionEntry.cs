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
        stringBuilder.AppendLine(" ---------------------------");
        if (string.IsNullOrEmpty(Key))
        {
            stringBuilder.Append($"EXECUTIONS: {Times} trivial runs (no observable input/state)");
        }
        else
        {
            stringBuilder.Append($" EXECUTE : {Key} ({Times} Times)");
        }
        return stringBuilder.ToString();
    }
}
