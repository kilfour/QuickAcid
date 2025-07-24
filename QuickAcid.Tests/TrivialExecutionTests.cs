using QuickAcid.Tests._Tools.ThePress;

namespace QuickAcid.Tests;

public class TrivialExecutionTests
{
    [Fact]
    public void FirstTry()
    {
        var counter = 0;
        var script =
            from act in "act".Act(() => { counter++; })
            from spec in "spec".Spec(() => counter != 5)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(10.ExecutionsPerRun()));

        Assert.Equal(1, article.Total().Actions());
        Assert.Equal("act", article.Execution(1).Action(1).Read().Label);
        Assert.Equal(5, article.Execution(1).Read().Times);
    }
}
