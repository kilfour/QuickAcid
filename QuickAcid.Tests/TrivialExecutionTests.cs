using QuickAcid.Reporting;

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
        var report = new QState(script).Observe(10);
        var entry = report.Single<ReportCollapsedExecutionEntry>();
        Assert.Equal("act", entry.Key);
        Assert.Equal(5, entry.Times);
    }
}
