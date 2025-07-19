using QuickAcid.Reporting;
using QuickFuzzr;

namespace QuickAcid.Tests.Bugs;

public class SpecIfNotEvaluated
{
    [Fact]
    public void Try_A_Number()
    {
        var script =
            from input in "input".Input(MGen.Constant(42))
            from act1 in "act1".Act(() => { return 5 * input; })
            from act2 in "act2".Act(() => { return 6 * input; })
            from spec in "spec".SpecIf(() => act1 != 0, () => act1 == act2)
            select Acid.Test;
        var ex = Assert.Throws<FalsifiableException>(() =>
            QState.Run(script)
                .WithOneRunAndOneExecution());
        var report = ex.QAcidReport;
        Assert.Single(report.OfType<ReportInputEntry>());
    }
}