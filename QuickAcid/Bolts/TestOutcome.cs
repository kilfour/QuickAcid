namespace QuickAcid.Bolts;

public abstract record TestOutcome
{
    public static readonly TestOutcome Success = new SuccessOutcome();
    public static readonly TestOutcome SpecFailed = new SpecFailedOutcome();
    public static readonly TestOutcome FailedWithException = new FailedWithExceptionOutcome();
    public sealed record SuccessOutcome : TestOutcome;
    public sealed record SpecFailedOutcome : TestOutcome;
    public sealed record FailedWithExceptionOutcome : TestOutcome;

    public bool Failed => this switch
    {
        SpecFailedOutcome => true,
        FailedWithExceptionOutcome => true,
        _ => false
    };

}
