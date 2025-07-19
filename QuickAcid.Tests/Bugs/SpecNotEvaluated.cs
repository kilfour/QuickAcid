using QuickAcid.Reporting;
using QuickFuzzr;

namespace QuickAcid.Tests.Bugs;

public class SpecNotEvaluated
{
    [Fact]
    public void Using_A_Value_That_Is_Not_Supposed_To_Be_Null_But_Shrinking_Tries_It_Anyway()
    {
        var script =
            from input in "input".Input(MGen.String())
            from act in "act".Act(() => { if (input == null) throw new Exception("Boom"); })
            from spec in "spec".Spec(() => false)
            select Acid.Test;
        var ex = Assert.Throws<FalsifiableException>(() =>
            QState.Run(script)
                .WithOneRunAndOneExecution());
        var report = ex.QAcidReport;
        Assert.Empty(report.OfType<ReportInputEntry>());
    }

    [Fact]
    public void Try_A_Number()
    {
        var script =
            from input in "input".Input(MGen.Constant(42))
            from act in "act".Act(() => { return 5 * input; })
            from spec in "spec".Spec(() => input != 42)
            select Acid.Test;
        var ex = Assert.Throws<FalsifiableException>(() =>
            QState.Run(script)
                .WithOneRunAndOneExecution());
        var report = ex.QAcidReport;
        Assert.Single(report.OfType<ReportInputEntry>());
    }


}