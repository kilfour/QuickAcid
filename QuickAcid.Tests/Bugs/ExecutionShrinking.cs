using QuickAcid.Reporting;
using QuickPulse.Arteries;

namespace QuickAcid.Tests.Bugs;

public class ExecutionShrinking
{
    [Fact]
    public void Choosing_Executions_Two_Need_To_Remain()
    {
        var script =
            from collector in "collector".Stashed(() => new TheCollector<int>())
            from ops in "ops".Choose(
                "act1".Act(() => collector.Flow(1)),
                "act2".Act(() => collector.Flow(2)),
                "act3".Act(() => collector.Flow(3))
            )
            from spec in "spec".Spec(() => collector.TheExhibit.Count == collector.TheExhibit.Distinct().Count())
            select Acid.Test;
        var ex = Assert.Throws<FalsifiableException>(() => QState.Run(script)
            .WithOneRun()
            .And(30.ExecutionsPerRun()));
        var report = ex.QAcidReport;
        Assert.Single(report.OfType<ReportCollapsedExecutionEntry>());
        var entry = report.Single<ReportCollapsedExecutionEntry>();
        Assert.Equal(2, entry.Times);
    }
}