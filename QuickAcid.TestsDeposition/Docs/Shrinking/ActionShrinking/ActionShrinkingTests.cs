using QuickAcid.Reporting;


namespace QuickAcid.TestsDeposition.Docs.Shrinking.ActionShrinking;

public class ActionShrinkingTests
{
    [Fact]
    public void FirstActionIrrelevant()
    {
        var script =
            from a1 in "a1".Act(() => { })
            from a2 in "a2".Act(() => throw new Exception("Boom"))
            select Acid.Test;
        var report = new QState(script).ShrinkingActions().ObserveOnce();
        Assert.NotNull(report);
        var entry = report.Single<ReportExecutionEntry>();
        Assert.Equal("a2", entry.Key);
    }

    [Fact]
    public void SecondActionIrrelevant()
    {
        var script =
            from a1 in "a1".Act(() => throw new Exception("BOOM"))
            from a2 in "a2".Act(() => { })
            select Acid.Test;
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();
        Assert.NotNull(report);
        var entry = report.Single<ReportExecutionEntry>();
        Assert.Equal("a1", entry.Key);
    }
}