
namespace QuickAcid.Proceedings;

public class CaseFile
{
    public List<ExecutionDeposition> ExecutionDepositions { get; } = [];

    public CaseFile AddExecutionDeposition(ExecutionDeposition executionDepostion)
    {
        ExecutionDepositions.Add(executionDepostion);
        return this;
    }
};