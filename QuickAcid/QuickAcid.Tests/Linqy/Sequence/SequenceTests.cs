using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;

namespace QuickAcid.Tests.Linqy.Sequence;

public class SequenceTests
{
    [Fact]
    public void TwoActionsExceptionThrownOnFirst()
    {
        var report = new QState(
                "foobar".Sequence(
                "foo".Act(() => throw new Exception()),
                "bar".Act(() => { })))
            .ObserveOnce();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
        Assert.NotNull(report.Exception);
    }

    [Fact]
    public void TwoActionsExceptionThrownOnSecond()
    {

        var run =
            "foobar".Sequence(
            "foo".Act(() => { }),
            "bar".Act(() => throw new Exception()));
        var report = new QState(run).Observe(2); ;
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("bar", entry.Key);
        Assert.NotNull(report.Exception);
    }
}