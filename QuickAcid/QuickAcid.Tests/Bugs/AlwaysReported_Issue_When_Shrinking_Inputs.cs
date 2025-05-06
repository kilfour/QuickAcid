using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Tests.Bugs;


public class AlwaysReported_Issue_When_Shrinking_Inputs
{
    public class Container { public int Value; }

    [Fact]
    public void AlwaysReported_input_should_not_get_polluted_by_shrinking()
    {
        var run =
           from container in "container".AlwaysReported(() => new Container { Value = 21 }, a => a.Value.ToString())
           from input in "input".Shrinkable(MGen.Constant(42))
           from _do in "do".Act(() => { container.Value = input; })
           from _ in "spec".Spec(() => container.Value != 42)
           select Acid.Test;

        var report = new QState(run).ObserveOnce();
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportAlwaysReportedInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("21", entry.Value);
    }
}