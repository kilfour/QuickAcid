using QuickAcid.Bolts;
using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.TheFortyNiners.Tests.Bugs;


public class AlwaysReported_Issue_When_Shrinking_Inputs
{
    public class Container { public int Value; }

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

        var entry = report.FirstOrDefault<ReportTrackedEntry>();
        Assert.NotNull(entry);
        Assert.Equal("21", entry.Value);
    }
}