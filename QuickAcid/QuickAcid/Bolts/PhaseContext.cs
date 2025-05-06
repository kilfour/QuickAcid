namespace QuickAcid.Bolts;

public class PhaseContext
{
    public bool Failed { get; set; }
    public bool BreakRun { get; set; }
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

    public void MarkFailure(string failingSpec)
    {
        Failed = true;
        FailingSpec = failingSpec;
    }

    public void MarkFailure(Exception exception, PhaseContext originalPhase)
    {
        if (phase == QAcidPhase.ShrinkingExecutions)
        {
            if (originalPhase.Exception == null)
            {
                BreakRun = true;
                Failed = true;
                return;
            }

            if (originalPhase.Exception.GetType() != exception.GetType())
            {
                BreakRun = true;
                Failed = true;
                return;
            }
        }
        Failed = true;
        Exception = exception;
    }
}
