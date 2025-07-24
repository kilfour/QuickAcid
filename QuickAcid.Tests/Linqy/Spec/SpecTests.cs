using QuickAcid.Tests._Tools.ThePress;

namespace QuickAcid.Tests.Linqy.Spec;

public class SpecTests
{
    [Fact]
    public void SpecOnlyReturnsTrue()
    {
        TheJournalist.Unearths(QState.Run("foo".Spec(() => true)).WithOneRunAndOneExecution());
    }

    [Fact]
    public void SpecOnlyReturnsFalse()
    {
        var script = "foo".Spec(() => false);

        var article = TheJournalist.Exposes(() =>
            QState.Run(script).WithOneRunAndOneExecution());

        Assert.Equal("foo", article.FailedSpec());
    }

    [Fact]
    public void SpecMultipleFirstFails()
    {
        var script =
            from __a in "foo".Act(() => { })
            from _s1 in "first failed".Spec(() => false)
            from _s2 in "second passed".Spec(() => true)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.Equal("first failed", article.FailedSpec());
    }

    [Fact]
    public void SpecMultipleSecondFails()
    {
        var script =
            from __a in "foo".Act(() => { })
            from _s1 in "first passed".Spec(() => true)
            from _s2 in "second failed".Spec(() => false)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.Equal("second failed", article.FailedSpec());
    }
}