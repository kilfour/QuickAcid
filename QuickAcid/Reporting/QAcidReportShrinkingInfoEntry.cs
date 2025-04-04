namespace QuickAcid.Reporting;

public class QAcidReportShrinkingInfoEntry : QAcidReportEntry
{
    public QAcidReportShrinkingInfoEntry(string key)
        : base(key) { }

    public int NrOfActions { get; set; }
    public int ShrinkCount { get; set; }

    public override string ToString()
    {
        return $"Falsified after {NrOfActions} actions, {ShrinkCount} shrinks";
    }
}
