using QuickAcid.Reporting;
using QuickAcid.Tests._Tools.ThePress;


namespace QuickAcid.TestsDeposition.Docs.Shrinking.ActionShrinking;

public class ActionShrinkingTests
{
    [Fact]
    public void FirstActionIrrelevant()
    {
        var script =
            from a1 in "a1".Act(() => { })
            from a2 in "a2".Act(() => throw new Exception("Boom"))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .Options(a => a with { ShrinkingActions = true })
            .WithOneRunAndOneExecution());

        Assert.Equal("a2", article.Execution(1).Action(1).Read().Label);
    }

    [Fact]
    public void SecondActionIrrelevant()
    {
        var script =
            from a1 in "a1".Act(() => throw new Exception("BOOM"))
            from a2 in "a2".Act(() => { })
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.Equal("a1", article.Execution(1).Action(1).Read().Label);
    }
}