using QuickAcid.Reporting;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;
using QuickMGenerate;

namespace QuickAcid.Tests.Bugs;

public class ChooseFromDynamicCollection
{
    [Fact]
    public void Example()
    {
        var run =
            from list in "list".Stashed(() => new List<int>() { 0 })
            from input in "input".Dynamic(MGen.Constant(list.Last()))
            from act in "act".Act(() => { list.Add(input + 1); })
            from spec in "spec".Spec(() => !list.Contains(2))
            select Acid.Test;

        var report = new QState(run).Observe(50);
        Assert.NotNull(report);

        var actEntries = report.OfType<ReportExecutionEntry>();
        Assert.Equal(2, actEntries.Count());
    }

    [Fact]
    public void Trickier()
    {
        var run =
            from list in "list".Stashed(() => new List<int>() { 0 })
            from input in "input".Dynamic(MGen.Constant(list.Last()))
            from act in "act".Act(() => { list.Add(input + 1); })
            from spec in "spec".Spec(() => !list.Contains(2))
            select Acid.Test;

        var report = new QState(run).Observe(50);
        Assert.NotNull(report);

        var actEntries = report.OfType<ReportExecutionEntry>();
        Assert.Equal(2, actEntries.Count());
    }
}
