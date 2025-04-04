using System.Text;

namespace QuickAcid.Reporting;

public class QAcidReportActEntry : QAcidReportEntry
{
    public Exception? Exception { get; set; }

    public QAcidReportActEntry(string key) : base(key) { }

    public QAcidReportActEntry(string key, Exception exception) : this(key)
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
