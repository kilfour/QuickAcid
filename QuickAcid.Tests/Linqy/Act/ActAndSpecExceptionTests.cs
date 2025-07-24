using QuickAcid.Bolts;
using QuickAcid.Tests._Tools.ThePress;

namespace QuickAcid.Tests.Linqy.Act;

public class ActAndSpecExceptionTests
{
    [Fact]
    public void ExceptionThrownByAct()
    {
        var script =
            from foo in "foo".Act(() => { if (true) throw new Exception(); })
            from spec in "spec".Spec(() => true)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());
        var entry = article.Execution(1).Action(1).Read();

        Assert.NotNull(article.Exception());
        Assert.Equal("foo", entry.Label);
    }

    [Fact]
    public void ExceptionThrownBySpecIsNotAQuickAcidException()
    {
        var script =
            from foo in "foo".Act(() => true)
            from spec in "spec".Spec(Throw)
            select Acid.Test;
        var ex = Assert.Throws<Exception>(() => QState.Run(script).WithOneRunAndOneExecution());
        Assert.IsNotType<FalsifiableException>(ex);
        Assert.Contains("QuickAcid.Tests.Linqy.Act.ActAndSpecExceptionTests.Throw()", ex.StackTrace);
    }

    // needs to be here because of contains assert above
    private bool Throw()
    {
        throw new Exception();
    }
}