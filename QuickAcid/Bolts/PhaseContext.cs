namespace QuickAcid.Bolts;

public class PhaseContext
{
    public bool Failed { get; set; }
    public bool BreakRun { get; set; }
    // public string? FailingSpec { get; set; }
    // public Exception? Exception { get; set; }

    public void Reset()
    {
        Failed = false;
        BreakRun = false;
        // FailingSpec = null;
        // Exception = null;
    }

    public void SpecFailed(string failingSpec)
    {
        // CurrentContext.Failed = true;
        // FailingSpec = failingSpec;
    }

    public void FailedWithException(Exception exception)
    {
        // if (CurrentPhase == QAcidPhase.ShrinkingExecutions)
        // {
        //     if (Exception == null)
        //     {
        //         CurrentContext.BreakRun = true;
        //         CurrentContext.Failed = true;
        //         return;
        //     }

        //     if (Exception.GetType() != exception.GetType())
        //     {
        //         CurrentContext.BreakRun = true;
        //         CurrentContext.Failed = true;
        //         return;
        //     }
        // }
        // CurrentContext.Failed = true;
        // Exception = exception;
    }
}
