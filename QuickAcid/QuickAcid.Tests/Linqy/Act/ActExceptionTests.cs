using QuickAcid.Reporting;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Tests.Linqy.Act;

public class ActExceptionTests
{
    [Fact]
    public void SimpleExceptionThrown()
    {
        var run = "foo".Act(() => { if (true) throw new Exception(); });
        var entry = new QState(run).ObserveOnce().FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
        Assert.NotNull(entry.Exception);
    }

    [Fact]
    public void TwoActionsExceptionThrownOnFirst()
    {
        var run =
            from foo in "foo".Act(() => throw new Exception())
            from bar in "bar".Act(() => { })
            select Acid.Test;
        var entry = new QState(run).ObserveOnce().FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
        Assert.NotNull(entry.Exception);
    }

    [Fact]
    public void TwoActionsExceptionThrownOnSecond()
    {
        var run =
            from foo in "foo".Act(() => { })
            from bar in "bar".Act(() => throw new Exception())
            select Acid.Test;
        var entry = new QState(run).ObserveOnce().FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo, bar", entry.Key);
        Assert.NotNull(entry.Exception);
    }
}
