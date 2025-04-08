namespace QuickAcid.Tests.Bugs;

using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
public class WhenTheHeartIsA
{
    [Fact]
    public void LonelyHunter()
    {
        var run =
            from obj in "needs ref".AlwaysReported(() => new object())
            from _ in "needs exception".Act(() => throw new Exception("boom"))
            from _s1 in "spec if".SpecIf(() => true, () => true)
            from _s2 in MinimalSpec(obj)
            select Acid.Test;
        var report = run.ReportIfFailed(30, 10);
        Assert.NotNull(report);
    }

    private static QAcidRunner<Acid> MinimalSpec(object obj)
    {
        return from _ in "always true".Spec(() => true) select Acid.Test;
    }
}
