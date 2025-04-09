using System.Text;

namespace QuickAcid.Bolts;

public class DecoratedValue
{
    public object? Value { get; set; }
    public bool IsIrrelevant { get; set; }
    public string? ReportingMessage { get; set; }
    public Func<object, string>? Stringify { get; set; }

    public string ToDiagnosticString()
    {
        var sb = new StringBuilder();
        if (Value == null)
            sb.Append("null");
        else
            sb.Append(Stringify == null ? Value : Stringify(Value));
        if (IsIrrelevant)
            sb.Append(" : Irrelevant");
        if (ReportingMessage != null)
            sb.AppendFormat(", {0}", ReportingMessage);
        return sb.ToString();
    }
}
