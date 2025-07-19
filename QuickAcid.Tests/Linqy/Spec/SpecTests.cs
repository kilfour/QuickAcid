using QuickAcid.Reporting;

namespace QuickAcid.Tests.Linqy.Spec;

public class SpecTests
{
    [Fact]
    public void SpecOnlyReturnsTrue()
    {
        Assert.True(QState.Run("foo".Spec(() => true)).WithOneRunAndOneExecution().IsSuccess);
    }

    [Fact]
    public void SpecOnlyReturnsFalse()
    {
        var report = new QState("foo".Spec(() => false)).ObserveOnce();
        var entry = report.Entries.OfType<ReportSpecEntry>().FirstOrDefault();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
    }

    [Fact]
    public void SpecMultipleFirstFails()
    {
        var script =
            from __a in "foo".Act(() => { })
            from _s1 in "first failed".Spec(() => false)
            from _s2 in "second passed".Spec(() => true)
            select Acid.Test;

        var entry = new QState(script).ObserveOnce().Single<ReportSpecEntry>();

        Assert.NotNull(entry);
        Assert.Equal("first failed", entry.Key);
    }

    [Fact]
    public void SpecMultipleSecondFails()
    {
        var script =
            from __a in "foo".Act(() => { })
            from _s1 in "first passed".Spec(() => true)
            from _s2 in "second failed".Spec(() => false)
            select Acid.Test;

        var entry = new QState(script).ObserveOnce().Single<ReportSpecEntry>();

        Assert.NotNull(entry);
        Assert.Equal("second failed", entry.Key);
    }
}