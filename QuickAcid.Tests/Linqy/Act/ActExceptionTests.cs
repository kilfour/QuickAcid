using QuickAcid.Reporting;

namespace QuickAcid.Tests.Linqy.Act;

public class ActExceptionTests
{
    [Fact]
    public void SimpleExceptionThrown()
    {
        var script = "foo".Act(() => { if (true) throw new Exception(); });
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
        Assert.NotNull(report.Exception);
    }

    [Fact]
    public void TwoActionsExceptionThrownOnFirst()
    {
        var script =
            from foo in "foo".Act(() => throw new Exception())
            from bar in "bar".Act(() => { })
            select Acid.Test;
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
        Assert.NotNull(report.Exception);
    }

    [Fact]
    public void TwoActionsExceptionThrownOnSecond()
    {
        var script =
            from foo in "foo".Act(() => { })
            from bar in "bar".Act(() => throw new Exception())
            select Acid.Test;
        var report = new QState(script).ShrinkingActions().ObserveOnce();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("bar", entry.Key);
        Assert.NotNull(report.Exception);
    }

    [Fact]
    public void Action_only_throws_on_second_execution()
    {
        var counter = 0;
        var exception = new Exception();
        var script =
            from _a1 in "c".ActIf(() => counter < 2, () => counter++)
            from _a2 in "act".ActIf(() => counter == 2, () => { throw new Exception("BOOM"); })
            select Acid.Test;
        var report = new QState(script).ShrinkingActions().Observe(3);

        Assert.NotNull(report);
        Assert.NotNull(report.Exception);
        Assert.Equal("BOOM", report.Exception.Message);

        Assert.Single(report.OfType<ReportExecutionEntry>());
        var entry = report.FirstOrDefault<ReportExecutionEntry>();

        Assert.NotNull(entry);
        Assert.Equal("act", entry.Key);
    }

    [Fact]
    public void Action_throws_different_after_first_run()
    {
        var counter = 0;
        var exception = new Exception("First");
        var script =
            from _a1 in "c".ActIf(() => counter < 2, () => counter++)
            from _a2 in "act".ActIf(() => counter == 2,
                () => { var exc = exception; exception = new InvalidOperationException(); throw exc; })
            select Acid.Test;
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .And(3.ExecutionsPerRun());

        Assert.NotNull(report);
        Assert.NotNull(report.Exception);
        Assert.Equal("First", report.Exception.Message);

        Assert.Equal(2, report.OfType<ReportExecutionEntry>().Count());
        var entry = report.SecondOrDefault<ReportExecutionEntry>();

        Assert.NotNull(entry);
        Assert.Equal("c, act", entry.Key);
    }
}
