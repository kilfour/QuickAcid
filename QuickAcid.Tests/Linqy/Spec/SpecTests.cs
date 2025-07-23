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
        var script = "foo".Spec(() => false);
        var report = QState.Run(script).Options(a => a with { DontThrow = true }).WithOneRunAndOneExecution();
        var entry = report.Entries.OfType<ReportSpecEntry>().FirstOrDefault();

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

        var entry = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun().Single<ReportSpecEntry>();


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

        var entry = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun().Single<ReportSpecEntry>();


        Assert.Equal("second failed", entry.Key);
    }
}