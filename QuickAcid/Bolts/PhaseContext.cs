namespace QuickAcid.Bolts;

public class PhaseContext
{
    public bool Failed { get; set; }
    public bool BreakRun { get; set; }
    public string? FailingSpec { get; set; }
    public Exception? Exception { get; set; }

    public void Reset()
    {
        Failed = false;
        BreakRun = false;
        FailingSpec = null;
        Exception = null;
    }

    public void SpecFailed(string failingSpec)
    {
        Failed = true;
        FailingSpec = failingSpec;
    }

    public void FailedWithException(Exception exception, bool IsShrinkingExecutions, PhaseContext originalPhase)
    {
        if (IsShrinkingExecutions)
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
