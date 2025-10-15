using QuickAcid.Tests._Tools.ThePress;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Linqy.Sequence;

public class SequenceTests
{
    [Fact]
    public void TwoActionsExceptionThrownOnFirst()
    {
        var script =
            "foobar".Sequence(
            "foo".Act(() => throw new Exception()),
            "bar".Act(() => { }));

        var article = TheJournalist.Exposes(() => QState.Run(script).WithOneRunAndOneExecution());

        Assert.Equal("foo", article.Execution(1).Action(1).Read().Label);
        Assert.NotNull(article.Exception());
    }

    [Fact]
    public void TwoActionsExceptionThrownOnSecond()
    {

        var script =
            "foobar".Sequence(
            "foo".Act(() => { }),
            "bar".Act(() => throw new Exception()));

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(2.ExecutionsPerRun()));

        Assert.Equal("bar", article.Execution(1).Action(1).Read().Label);
        Assert.NotNull(article.Exception());


    }
}