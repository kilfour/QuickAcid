using System.Text;

namespace QuickAcid.Reporting;

public class ReportActEntry : QAcidReportEntry
{
    public Exception? Exception { get; set; }

    public ReportActEntry(string key) : base(key) { }

    public ReportActEntry(string key, Exception exception) : this(key)
    {
        Exception = exception;
    }

    public override string ToString()
    {
        var stringBuilder = new StringBuilder($"Execute : {Key}");
        stringBuilder.AppendLine();
        if (Exception != null)
        {
            stringBuilder.Append(Exception);
            stringBuilder.AppendLine();
        }
        stringBuilder.Append(" ---------------------------");
        return stringBuilder.ToString();
    }
}
