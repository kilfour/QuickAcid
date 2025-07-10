using QuickAcid.Reporting;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

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
        var report = new QState(script).ObserveOnce();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.NotNull(report.Exception);
        Assert.Equal("foo", entry.Key);
    }

    [Fact]
    public void ExceptionThrownBySpecIsNotAQuickAcidException()
    {
        var script =
            from foo in "foo".Act(() => true)
            from spec in "spec".Spec(Throw)
            select Acid.Test;
        var ex = Assert.Throws<Exception>(() => new QState(script).TestifyOnce());
        Assert.IsNotType<FalsifiableException>(ex);
        Assert.Contains("QuickAcid.Tests.Linqy.Act.ActAndSpecExceptionTests.Throw()", ex.StackTrace);
    }

    // needs to be here because of contains assert above
    private bool Throw()
    {
        throw new Exception();
    }
}