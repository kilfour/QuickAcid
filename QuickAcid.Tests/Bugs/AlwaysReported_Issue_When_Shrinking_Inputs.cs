using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickAcid.Strike;
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

        var report = run.ReportIfFailed(1, 1);
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportAlwaysReportedInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("21", entry.Value);
    }

    [Fact]
    public void AlwaysReported_input_should_not_get_polluted_by_shrinking_STRIKE()
    {
        var ex = Assert.Throws<FalsifiableException>(() =>
            Test.This(() => new Container { Value = 21 }, a => a.Value.ToString())
                .Arrange(("input", MGen.Constant(42)))
                .Act(Perform.This("input", (Container container, int input) => { container.Value = input; }))
                .Assert("spec", container => container.Value != 42)
                .UnitRun()
        );
        var report = ex.QAcidReport;
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportAlwaysReportedInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("21", entry.Value);
    }

    [Fact]
    public void AlwaysReported_input_should_not_get_polluted_by_shrinking_Fluent()
    {
        var ex = Assert.Throws<FalsifiableException>(() =>
            SystemSpecs.Define()
                .AlwaysReported("container", () => new Container { Value = 21 }, a => a.Value.ToString())
                .Fuzzed("input", MGen.Constant(42))
                .Do("do", ctx => { ctx.GetItAtYourOwnRisk<Container>("container").Value = ctx.GetItAtYourOwnRisk<int>("input"); })
                .Assert("spec", ctx => ctx.GetItAtYourOwnRisk<Container>("container").Value != 42)
                .DumpItInAcid()
                .ThrowFalsifiableExceptionIfFailed(1, 1)
        );
        var report = ex.QAcidReport;
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportAlwaysReportedInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("21", entry.Value);
    }
}