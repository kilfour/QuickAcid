using QuickFuzzr.UnderTheHood;

namespace QuickAcid.Tests.Bugs;

public class DocumentingBehavior
{
    public Generator<int> Counter()
    {
        var counter = 0;
        return state => new Result<int>(counter++, state);
    }



    [Fact]
    public void ShrinkingInputs_phase_should_not_clear_failure()
    {
        var script =
            from act in "fail".Act(() => throw new Exception("boom"))
            select Acid.Test;

        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();
        Assert.NotNull(report);
        Assert.Contains("boom", report.Exception!.Message);
    }

    [Fact]
    public void Only_failing_spec_should_be_retried_on_consecutive_runs()
    {
        var secondSpecFailed = false;

        var script =
            from _ in "first".Spec(() => { if (secondSpecFailed) throw new Exception("BOOM"); return true; })
            from _2 in "second".Spec(() => { secondSpecFailed = true; return false; })
            select Acid.Test;

        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();

        Assert.Equal("second", report.GetSpecEntry().Key);
    }
}