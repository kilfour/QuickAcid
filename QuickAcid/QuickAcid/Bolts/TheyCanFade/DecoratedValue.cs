using QuickAcid.Bolts.ShrinkStrats;

namespace QuickAcid.Bolts.TheyCanFade;

public class DecoratedValue
{
    public object? Value { get; set; }
    public ShrinkOutcome? ShrinkOutcome { get; set; }
    public ReportingIntent ReportingIntent { get; set; } = ReportingIntent.Shrinkable;
    // ---------------------------------------------------------------------------------------
    // -- DEEP COPY
    public DecoratedValue DeepCopy()
    {
        return new DecoratedValue
        {
            Value = this.Value, // shallow copy — replace if needed
            ShrinkOutcome = this.ShrinkOutcome, // assuming immutable or value-type
            ReportingIntent = this.ReportingIntent
        };
    }
    // ---------------------------------------------------------------------------------------

    public List<ShrinkTrace> ShrinkTraces { get; init; } = [];
}

public enum ReportingIntent
{
    Shrinkable,   // only include if shrink contributed
    Always,       // always include (e.g., Assay, Touchstone)
    Never         // captured, never include
}