using System.Diagnostics;
using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class ExecutionArticle : AbstractArticle<ExecutionDeposition>
{
    public ExecutionArticle(ExecutionDeposition deposition)
        : base(deposition) { }

    [StackTraceHidden]
    public TrackedArticle Tracked(int number)
    {
        return new TrackedArticle(deposition.TrackedDepositions[number - 1]);
    }

    [StackTraceHidden]
    public ActionArticle Action(int number)
    {
        return new ActionArticle(deposition.ActionDepositions[number - 1]);
    }

    [StackTraceHidden]
    public InputArticle Input(int number)
    {
        return new InputArticle(deposition.InputDepositions[number - 1]);
    }

    [StackTraceHidden]
    public TraceArticle Trace(int number)
    {
        return new TraceArticle(deposition.TraceDepositions[number - 1]);
    }
}

