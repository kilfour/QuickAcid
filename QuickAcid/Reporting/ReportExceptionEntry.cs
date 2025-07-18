using System.Text;

namespace QuickAcid.Reporting;

public class ReportExceptionEntry : IAmAReportEntry
{
    private readonly Exception exception;

    public ReportExceptionEntry(Exception exception)
    {
        this.exception = exception;
    }

    public override string ToString()
    {
        var text = $"  ❌ Exception Thrown: {exception}";
        var length = text.Split(Environment.NewLine).Max(a => a.Length);
        if (length > 80)
            length = 80;
        var line = " " + new string('═', length);
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine(line);
        stringBuilder.AppendLine(text);
        stringBuilder.Append(line);
        return stringBuilder.ToString();
    }
}
