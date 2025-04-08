using QuickAcid.Nuts.Bolts;
using QuickAcid.Reporting;

namespace QuickAcid.Tests;

public class SequenceTests
{
    [Fact]
    public void TwoActionsExceptionThrownOnFirst()
    {
        var report =
            "foobar".Sequence(
            "foo".Act(() => throw new Exception()),
            "bar".Act(() => { })).ReportIfFailed();
        var entry = report.FirstOrDefault<ReportActEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
        Assert.NotNull(entry.Exception);
    }

    [Fact]
    public void TwoActionsExceptionThrownOnSecond()
    {
        var report =
            "foobar".Sequence(
            "foo".Act(() => { }),
            "bar".Act(() => throw new Exception()))
                .ReportIfFailed(1, 2);
        var entry = report.FirstOrDefault<ReportActEntry>();
        Assert.NotNull(entry);
        Assert.Equal("bar", entry.Key);
        Assert.NotNull(entry.Exception);
    }
}