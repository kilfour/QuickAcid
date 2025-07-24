namespace QuickAcid.Shrinking;

public abstract record ShrinkOutcome
{
    public static readonly ShrinkOutcome Irrelevant = new IrrelevantOutcome();
    public static ShrinkOutcome Report(string message) => new ReportedOutcome(message);

    public sealed record IrrelevantOutcome : ShrinkOutcome;
    public sealed record ReportedOutcome(string Message) : ShrinkOutcome;

    public override string ToString() => this switch
    {
        ReportedOutcome r => r.Message,
        IrrelevantOutcome => "Irrelevant",
        _ => "Unknown"
    };
}
