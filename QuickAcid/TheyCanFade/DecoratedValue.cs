using QuickAcid;
using QuickAcid.Shrinking;

namespace QuickAcid.TheyCanFade;

public class DecoratedValue
{
    public object? Value { get; set; }

    public ReportingIntent ReportingIntent { get; set; } = ReportingIntent.Shrinkable;

    private ShrinkTraceRecord root = new ShrinkTraceRecord();

    public void SetShrinkKind(ShrinkKind shrinkKind) { root.ShrinkKind = shrinkKind; }

    public void AddTrace(ShrinkKind shrinkKind, ShrinkTrace trace)
    {
        var parts = trace.Key.Split('.');
        var current = root;

        foreach (var part in parts.Skip(1))
        {
            if (!current.Children.TryGetValue(part, out var child))
            {
                child = new ShrinkTraceRecord();
                current.Children[part] = child;
            }
            current = child;
        }
        if (shrinkKind != ShrinkKind.KeepSameKind)
            current.ShrinkKind = shrinkKind;
        current.Traces.Add(trace);
    }

    public virtual List<ShrinkTrace> GetShrinkTraces()
    {
        return FlattenTraces(root);
    }

    public static List<ShrinkTrace> FlattenTraces(ShrinkTraceRecord record)
    {
        var result = new List<ShrinkTrace>();
        Flatten(record, "", result);
        return result;
    }

    private static void Flatten(ShrinkTraceRecord record, string prefix, List<ShrinkTrace> result)
    {
        foreach (var trace in record.Traces)
        {
            // Override Key with the fully qualified key path
            result.Add(trace); // with { Key = prefix.TrimEnd('.') }
        }

        foreach (var kvp in record.Children)
        {
            //var childKey = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";
            Flatten(kvp.Value, kvp.Key, result);
        }
    }

    public ShrinkOutcome GetShrinkOutcome()
    {
        var shrinkString = root.GetShrinkReportString();
        if (string.IsNullOrEmpty(shrinkString))
            return ShrinkOutcome.Irrelevant;
        return ShrinkOutcome.Report(shrinkString);
    }
}
