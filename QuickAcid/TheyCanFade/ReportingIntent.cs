namespace QuickAcid.TheyCanFade;

public enum ReportingIntent
{
    Shrinkable,   // only include if shrink contributed
    Always,       // always include (e.g., Assay, Touchstone)
    Never         // captured, never include
}