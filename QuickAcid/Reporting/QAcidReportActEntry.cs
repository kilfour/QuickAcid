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
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(" ---------------------------");
        stringBuilder.Append($" EXECUTE : {Key}");
        if (Exception != null)
        {
            stringBuilder.AppendLine();
            stringBuilder.Append(Exception);
        }
        return stringBuilder.ToString();
    }
}
