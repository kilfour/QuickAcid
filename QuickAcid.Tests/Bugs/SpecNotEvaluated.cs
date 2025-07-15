using QuickAcid.Reporting;
using QuickMGenerate;

namespace QuickAcid.Tests.Bugs;

public class SpecNotEvaluated
{
    public class Simples { public string? Name { get; set; } }

    [Fact]
    public void Foo()
    {
        var script =
            from input in "input".Input(MGen.One<Simples>())
            from act in "act".Act(() => input.Name!.ToString())
            from spec in "spec".Spec(() => false)
            select Acid.Test;
        var ex = Assert.Throws<FalsifiableException>(() =>
            QState.Run(script)
                .WithOneRunAndOneExecution());
        var report = ex.QAcidReport;
        Assert.Empty(report.OfType<ReportInputEntry>());
    }
}