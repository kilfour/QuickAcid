using QuickAcid.Bolts;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Bugs;


public class WhenTheHeartIsA
{
    [Fact]
    public void LonelyHunter()
    {
        var script =
            from obj in "needs ref".Tracked(() => new object())
            from _ in "needs exception".Act(() => throw new Exception("boom"))
            from _s1 in "spec if".SpecIf(() => true, () => true)
            from _s2 in MinimalSpec(obj)
            select Acid.Test;

        Assert.Throws<FalsifiableException>(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());
    }

    private static QAcidScript<Acid> MinimalSpec(object obj)
    {
        return from _ in "always true".Spec(() => true) select Acid.Test;
    }
}
