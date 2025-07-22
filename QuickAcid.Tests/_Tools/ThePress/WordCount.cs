using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class WordCount
{
    private readonly List<ExecutionDeposition> depositions;

    public WordCount(List<ExecutionDeposition> depositions)
    {
        this.depositions = depositions;
    }

    public int Executions()
    {
        return depositions.Count;
    }

    public int Inputs()
    {
        return depositions.Sum(a => a.InputDepositions.Count);
    }
}