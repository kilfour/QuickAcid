using QuickAcid.Reporting;
using QuickAcid.Tests._Tools.ThePress;

namespace QuickAcid.Tests.Linqy.Act;

public class ActExceptionTests
{
    [Fact]
    public void SimpleExceptionThrown()
    {
        var script = "foo".Act(() => { if (true) throw new Exception(); });

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var entry = article.Execution(1).Action(1).Read();

        Assert.Equal("foo", entry.Label);
        Assert.NotNull(article.Exception());
    }

    [Fact]
    public void TwoActionsExceptionThrownOnFirst()
    {
        var script =
            from foo in "foo".Act(() => throw new Exception())
            from bar in "bar".Act(() => { })
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var entry = article.Execution(1).Action(1).Read();

        Assert.Equal("foo", entry.Label);
        Assert.NotNull(article.Exception());
    }

    [Fact]
    public void TwoActionsExceptionThrownOnSecond()
    {
        var script =
            from foo in "foo".Act(() => { })
            from bar in "bar".Act(() => throw new Exception())
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .Options(a => a with { ShrinkingActions = true })
            .WithOneRunAndOneExecution());

        var entry = article.Execution(1).Action(1).Read();

        Assert.Equal("bar", entry.Label);
        Assert.NotNull(article.Exception());
    }

    [Fact]
    public void Action_only_throws_on_second_execution()
    {
        var counter = 0;
        var exception = new Exception();
        var script =
            from _a1 in "c".ActIf(() => counter < 2, () => counter++)
            from _a2 in "act".ActIf(() => counter == 2, () => { throw new Exception("BOOM"); })
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .Options(a => a with { ShrinkingActions = true })
            .WithOneRun()
            .And(3.ExecutionsPerRun()));


        Assert.NotNull(article.Exception());
        Assert.Equal("BOOM", article.Exception().Message);

        Assert.Equal(1, article.Total().Actions());
        var entry = article.Execution(1).Action(1).Read();


        Assert.Equal("act", entry.Label);
    }

    [Fact]
    public void Action_throws_different_after_first_run()
    {
        var counter = 0;
        var exception = new Exception("First");
        var script =
            from _a1 in "c".ActIf(() => counter < 2, () => counter++)
            from _a2 in "act".ActIf(() => counter == 2,
                () => { var exc = exception; exception = new InvalidOperationException(); throw exc; })
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(3.ExecutionsPerRun()));

        Assert.NotNull(article.Exception());
        Assert.Equal("First", article.Exception().Message);
        Assert.Equal(3, article.Total().Actions());
        Assert.Equal("c", article.Execution(1).Action(1).Read().Label);
        Assert.Equal("c", article.Execution(2).Action(1).Read().Label);
        Assert.Equal("act", article.Execution(2).Action(2).Read().Label);
    }
}
