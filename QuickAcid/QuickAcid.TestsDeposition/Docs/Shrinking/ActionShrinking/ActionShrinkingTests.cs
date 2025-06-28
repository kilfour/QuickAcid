using QuickAcid.Reporting;
using QuickPulse.Arteries;
using QuickPulse.Instruments;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.ActionShrinking;

public class ActionShrinkingTests
{
    [Fact]
    public void FirstActionIrrelevant()
    {
        var script =
            from a1 in "a1".Act(() => { })
            from a2 in "a2".Act(ComputerSays.No)
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);
        var entry = report.Single<ReportExecutionEntry>();
        Assert.Equal("a2", entry.Key);
    }

    [Fact]
    public void SecondActionIrrelevant()
    {
        var script =
            from a1 in "a1".Act(ComputerSays.No)
            from a2 in "a2".Act(() => { })
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);
        var entry = report.Single<ReportExecutionEntry>();
        Assert.Equal("a1", entry.Key);
    }
}