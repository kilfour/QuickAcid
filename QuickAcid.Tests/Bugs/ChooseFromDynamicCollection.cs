using QuickAcid.Reporting;
using QuickFuzzr;

namespace QuickAcid.Tests.Bugs;

public class ChooseFromDynamicCollection
{
    [Fact]
    public void Example()
    {
        var script =
            from list in "list".Stashed(() => new List<int>() { 0 })
            from input in "input".Derived(Fuzz.Constant(list.Last()))
            from act in "act".Act(() => { list.Add(input + 1); })
            from spec in "spec".Spec(() => !list.Contains(2))
            select Acid.Test;

        var report = new QState(script).Observe(50);
        Assert.NotNull(report);

        var entry = report.Single<ReportCollapsedExecutionEntry>();
        Assert.Equal("act", entry.Key);
        Assert.Equal(2, entry.Times);
    }

    [Fact]
    public void Trickier()
    {
        var script =
            from list in "list".Stashed(() => new List<int>() { 0 })
            from input in "input".Derived(Fuzz.Constant(list.Last()))
            from act in "act".Act(() => { list.Add(input + 1); })
            from spec in "spec".Spec(() => !list.Contains(2))
            select Acid.Test;

        var report = new QState(script).Observe(50);
        Assert.NotNull(report);

        var entry = report.Single<ReportCollapsedExecutionEntry>();
        Assert.Equal("act", entry.Key);
        Assert.Equal(2, entry.Times);
    }
}
