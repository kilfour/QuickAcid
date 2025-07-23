using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;

namespace QuickAcid.Tests.Bugs;

public class ChooseFromDynamicCollection
{
    [Fact]
    public void Example()
    {
        var script =
            from list in "list".Stashed(() => new List<int>() { 0 })
            from input in "input".Derived(Fuzz.Constant(list.Last()))
            from act in "act".Act(() => { list.Add(input + 1); })
            from spec in "spec".Spec(() => !list.Contains(2))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        Assert.Equal(2, article.Execution(1).Read().Times);
        Assert.Equal(1, article.Total().Executions());
        var actionDeposition = article.Execution(1).Action(1).Read();
        Assert.Equal("act", actionDeposition.Label);
    }

    [Fact]
    public void Trickier()
    {
        var script =
            from list in "list".Stashed(() => new List<int>() { 0 })
            from input in "input".Derived(Fuzz.Constant(list.Last()))
            from act in "act".Act(() => { list.Add(input + 1); })
            from spec in "spec".Spec(() => !list.Contains(2))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun()));

        Assert.Equal(2, article.Execution(1).Read().Times);
        Assert.Equal(1, article.Total().Executions());
        var actionDeposition = article.Execution(1).Action(1).Read();
        Assert.Equal("act", actionDeposition.Label);
    }
}
