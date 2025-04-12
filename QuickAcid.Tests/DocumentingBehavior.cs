
using QuickAcid;
using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate.UnderTheHood;

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
        // 🧪 Verifies that AlwaysReported(...) is stable across executions in the same run
        // Fails if Memory.ResetAllRunInputs() is called per execution instead of per run
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
        run.ReportIfFailed(1, 3);
        Assert.False(alwaysReportedChanged);
    }
}