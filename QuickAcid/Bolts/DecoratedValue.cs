using System.Text;

namespace QuickAcid.Bolts;

public class DecoratedValue
{
    public object? Value { get; set; }
    public bool IsIrrelevant { get; set; }
    public string? ReportingMessage { get; set; }

}
