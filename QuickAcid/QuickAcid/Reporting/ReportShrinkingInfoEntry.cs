namespace QuickAcid.Reporting;

public class ReportShrinkingInfoEntry : IAmAReportEntry
{
    public int NrOfActions { get; set; }
    public int ShrinkCount { get; set; }

    public int OriginalActionCount { get; set; } // optional

    public override string ToString()
    {
        if (OriginalActionCount > 0 && OriginalActionCount != NrOfActions)
        {
            return $"Falsified after {OriginalActionCount} action(s) → shrunk to {NrOfActions} action(s) in {ShrinkCount} steps";
        }

        return $"Minimal failing case: {NrOfActions} action(s) after {ShrinkCount} shrink(s)";
    }
}
