using QuickAcid.Reporting;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Tests.Linqy.Act;

public class ActAndSpecExceptionTests
{
    [Fact]
    public void ExceptionThrownByAct()
    {
        var run =
            from foo in "foo".Act(() => { if (true) throw new Exception(); })
            from spec in "spec".Spec(() => true)
            select Acid.Test;
        var report = run.ReportIfFailed();
        var entry = report.FirstOrDefault<ReportActEntry>();
        Assert.NotNull(entry);
        Assert.NotNull(entry.Exception);
        Assert.Equal("foo", entry.Key);
    }

    [Fact]
    public void ExceptionThrownBySpecIsNotAQuickAcidException()
    {
        var run =
            from foo in "foo".Act(() => true)
            from spec in "spec".Spec(Throw)
            select Acid.Test;
        var ex = Assert.Throws<Exception>(() => new QState(run).TestifyOnce().ThrowIfFailed());
        Assert.IsNotType<FalsifiableException>(ex);
        Assert.Contains("QuickAcid.Tests.Linqy.Act.ActAndSpecExceptionTests.Throw()", ex.StackTrace);
    }

    // needs to be here because of contains assert above
    private bool Throw()
    {
        throw new Exception();
    }
}