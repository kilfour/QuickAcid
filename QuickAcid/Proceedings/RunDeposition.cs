namespace QuickAcid.Proceedings;

public class RunDeposition
{
    public string RunLabel { get; } = string.Empty;
    public List<ExecutionDeposition> ExecutionDepositions { get; } = [];

    public RunDeposition() { }

    public RunDeposition(string runLabel)
    {
        RunLabel = runLabel;
    }

    public RunDeposition AddExecutionDeposition(ExecutionDeposition executionDepostion)
    {
        ExecutionDepositions.Add(executionDepostion);
        return this;
    }
};
