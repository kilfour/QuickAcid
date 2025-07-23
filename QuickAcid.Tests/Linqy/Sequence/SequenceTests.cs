using QuickAcid.Reporting;

namespace QuickAcid.Tests.Linqy.Sequence;

public class SequenceTests
{
    [Fact]
    public void TwoActionsExceptionThrownOnFirst()
    {
        var script =
            "foobar".Sequence(
            "foo".Act(() => throw new Exception()),
            "bar".Act(() => { }));
        var report = QState.Run(script).Options(a => a with { DontThrow = true }).WithOneRunAndOneExecution();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();

        Assert.Equal("foo", entry.Key);
        Assert.NotNull(report.Exception);
    }

    [Fact]
    public void TwoActionsExceptionThrownOnSecond()
    {

        var script =
            "foobar".Sequence(
            "foo".Act(() => { }),
            "bar".Act(() => throw new Exception()));
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .And(2.ExecutionsPerRun()); ;
        var entry = report.FirstOrDefault<ReportExecutionEntry>();

        Assert.Equal("bar", entry.Key);
        Assert.NotNull(report.Exception);
    }
}