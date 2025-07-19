namespace QuickAcid.Proceedings;

public class Verdict
{
    public int OriginalRunExecutionCount { get; init; } = 0;
    public int ExecutionCount { get; init; } = 0;
    public int ShrinkCount { get; init; } = 0;
    public int Seed { get; init; } = 0;
    public FailureDeposition FailureDeposition { get; }

    public List<ExecutionDeposition> ExecutionDepositions { get; } = [];

    public Verdict(FailureDeposition failureDeposition) { FailureDeposition = failureDeposition; }

    public Verdict AddExecutionDeposition(ExecutionDeposition executionDepostion)
    {
        ExecutionDepositions.Add(executionDepostion);
        return this;
    }
}
