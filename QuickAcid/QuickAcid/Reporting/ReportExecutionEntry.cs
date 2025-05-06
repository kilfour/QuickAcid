using System.Text;

namespace QuickAcid.Reporting;

public class ReportExecutionEntry : ReportEntry
{
    public Exception? Exception { get; set; }

    public ReportExecutionEntry(string key) : base(key) { }

    public ReportExecutionEntry(string key, Exception exception) : this(key)
    {
        Exception = exception;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(" ---------------------------");
        stringBuilder.Append($" EXECUTE : {Key}");
        // if (Exception != null)
        // {
        //     stringBuilder.AppendLine();
        //     //stringBuilder.Append(Exception);
        // }
        return stringBuilder.ToString();
    }
}
