using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class WordCount(List<ExecutionDeposition> depositions)
{
    private readonly List<ExecutionDeposition> depositions = depositions;

    public int Executions()
    {
        return depositions.Count;
    }

    public int Actions()
    {
        return depositions.Sum(a => a.ActionDepositions.Count);
    }

    public int Inputs()
    {
        return depositions.Sum(a => a.InputDepositions.Count);
    }
}