using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate.UnderTheHood;

namespace QuickAcid.Tests.Bugs;

public class DocumentingBehavior
{
    public Generator<int> Counter()
    {
        var counter = 0;
        return state => new Result<int>(counter++, state);
    }

    [Fact]
    public void AlwaysReported_should_be_consistent_across_executions()
    {
        var executionCount = 0;
        var specHolds = true;
        var alwaysReportedChanged = false;
        var run =
            from token in "token".AlwaysReported(() =>
            {
                if (specHolds && executionCount == 1)
                    alwaysReportedChanged = true;
                executionCount = 0;
                specHolds = true;
                return 42;
            })
            from observe in "observe".Act(() =>
            {
                executionCount++;
            })
            from delayedFail in "fail".Spec(() =>
                {
                    specHolds = executionCount <= 2;
                    return specHolds;
                }
            )
            select Acid.Test;

        new QState(run).Observe(3);
        Assert.False(alwaysReportedChanged);
    }

    [Fact]
    public void ShrinkingInputs_phase_should_not_clear_failure()
    {
        var report =
            SystemSpecs
                .Define()
                .Do("fail", () => throw new Exception("boom"))
                .DumpItInAcid()
                .AndCheckForGold(1, 3);

        Assert.NotNull(report);
        Assert.Contains("boom", report.ToString());
    }

    [Fact]
    public void Only_failing_spec_should_be_retried_on_consecutive_runs()
    {
        var secondSpecFailed = false;

        var run =
            from _ in "first".Spec(() => { if (secondSpecFailed) throw new Exception("BOOM"); return true; })
            from _2 in "second".Spec(() => { secondSpecFailed = true; return false; })
            select Acid.Test;

        var report = new QState(run).ObserveOnce();

        Assert.Equal("second", report.GetSpecEntry().Key);
    }
}