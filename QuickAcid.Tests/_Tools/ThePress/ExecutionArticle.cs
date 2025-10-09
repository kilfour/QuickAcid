using System.Diagnostics;
using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class ExecutionArticle : AbstractArticle<ExecutionDeposition>
{
    public ExecutionArticle(ExecutionDeposition deposition)
        : base(deposition) { }

    [StackTraceHidden]
    public StashedArticle Tracked(int number)
        => new(deposition.TrackedDepositions[number - 1]);

    [StackTraceHidden]
    public ActionArticle Action(int number)
        => new(deposition.ActionDepositions[number - 1]);

    [StackTraceHidden]
    public InputArticle Input(int number)
        => new(deposition.InputDepositions[number - 1]);

    [StackTraceHidden]
    public TraceArticle Trace(int number)
        => new(deposition.TraceDepositions[number - 1]);
}
