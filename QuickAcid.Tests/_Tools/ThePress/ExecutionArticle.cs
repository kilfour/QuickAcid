using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class ExecutionArticle : AbstractArticle<ExecutionDeposition>
{
    public ExecutionArticle(ExecutionDeposition deposition)
        : base(deposition) { }

    public TrackedArticle Tracked(int number)
    {
        return new TrackedArticle(deposition.TrackedDepositions[number - 1]);
    }

    public ActionArticle Action(int number)
    {
        return new ActionArticle(deposition.ActionDepositions[number - 1]);
    }

    public InputArticle Input(int number)
    {
        return new InputArticle(deposition.InputDepositions[number - 1]);
    }
}
