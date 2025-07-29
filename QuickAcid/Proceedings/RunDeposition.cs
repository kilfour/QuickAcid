namespace QuickAcid.Proceedings;

public class RunDeposition
{
    public string Label { get; } = string.Empty;
    public List<ExecutionDeposition> ExecutionDepositions { get; } = [];

    public RunDeposition(string runLabel)
    {
        Label = runLabel;
    }

    public RunDeposition AddExecutionDeposition(ExecutionDeposition executionDepostion)
    {
        ExecutionDepositions.Add(executionDepostion);
        return this;
    }
};
