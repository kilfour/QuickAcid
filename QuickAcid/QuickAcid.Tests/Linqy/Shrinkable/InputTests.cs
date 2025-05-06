using QuickAcid.Reporting;
using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;


namespace QuickAcid.Tests.Linqy.Shrinkable;

public class InputTests
{
    [Fact]
    public void UnusedInputIsNotReported()
    {
        var run =
            from input in "input".Shrinkable(MGen.Int())
            from foo in "foo".Act(() =>
            {
                if (true) throw new Exception();
            })
            select Acid.Test;
        var report = new QState(run).ObserveOnce();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("foo", entry.Key);
        Assert.NotNull(entry.Exception);
    }
}