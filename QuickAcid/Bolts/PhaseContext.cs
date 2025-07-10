namespace QuickAcid.Bolts;

public class PhaseContext
{
    public bool Failed { get; set; }
    private bool BreakRun;
    public string? FailingSpec { get; set; }
    public Exception? Exception { get; set; }
    private readonly QAcidPhase phase;

    public PhaseContext(QAcidPhase phase) => this.phase = phase;

    public void Reset()
    {
        Failed = false;
        BreakRun = false;
        FailingSpec = null;
        Exception = null;
    }

    public bool NeedsToStop() => BreakRun || Failed;

    internal void StopRun() => BreakRun = true;

    public void MarkFailure(string failingSpec)
    {
        Failed = true;
        FailingSpec = failingSpec;
    }

    public void MarkFailure(Exception exception, PhaseContext originalPhase)
    {
        if (phase != QAcidPhase.NormalRun)
        {
            if (IsExceptionMismatch(exception, originalPhase))
            {
                // Not marking as Failed: shrink result is invalid (mismatch), but run can't continue
                BreakRun = true;
                return;
            }
        }
        Failed = true;
        Exception = exception;
    }

    private static bool IsExceptionMismatch(Exception actual, PhaseContext expected)
    {
        if (expected.Exception == null) return true;

        var e = expected.Exception;
        return actual.GetType() != e.GetType()
            || actual.Message != e.Message
            || (actual.InnerException?.GetType() != e.InnerException?.GetType());
    }


    // private static bool IsExceptionMismatch(Exception exception, PhaseContext originalPhase)
    // {
    //     return originalPhase.Exception == null || originalPhase.Exception.GetType() != exception.GetType();
    // }
}
