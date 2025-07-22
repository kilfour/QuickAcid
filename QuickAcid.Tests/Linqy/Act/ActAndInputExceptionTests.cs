using QuickAcid.Reporting;
using QuickFuzzr;

namespace QuickAcid.Tests.Linqy.Act;

public class ActAndInputExceptionTests
{
    [Fact]
    public void ExceptionThrownByAct()
    {
        var script =
            from input in "input".Input(Fuzz.Int(1, 1))
            from foo in "foo".Act(() => { if (input == 1) throw new Exception(); })
            from spec in "spec".Spec(() => true)
            select Acid.Test;

        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();

        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Key);

        var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(actEntry);
        Assert.Equal("foo", actEntry.Key);
    }
}