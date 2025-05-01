using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts.TheyCanFade;

public class DecoratedValue
{
    public object? Value { get; set; }
    public ShrinkOutcome? ShrinkOutcome { get; set; }

    public ReportingIntent ReportingIntent { get; set; } = ReportingIntent.Shrinkable;
}

public enum ReportingIntent
{
    Shrinkable,   // only include if shrink contributed
    Always,       // always include (e.g., Assay, Touchstone)
    Never         // captured, never include
}