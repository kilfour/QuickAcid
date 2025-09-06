using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;

namespace QuickAcid.Tests;

public class SeedTests
{
    [Fact]
    public void Seed_Shows_Up_In_Report()
    {
        var script =
            from act in "act".Act(() => { })
            from spec in "spec".Spec(() => false)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script, 42)
            .WithOneRun()
            .And(10.ExecutionsPerRun()));


        Assert.Equal(42, article.Seed());
    }

    [Fact(Skip = "Works, but the shrink executions 3 times hack, messes with current Assert value.")]
    public void Seed_Allows_Reruns()
    {
        var collector = new List<int>();
        var script =
            from input in "input".Input(Fuzz.Int())
            from act in "act".Act(() => collector.Add(input))
            from spec in "spec".Spec(() => collector.Count != 2)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script, 42).WithOneRun().And(10.ExecutionsPerRun()));

        Assert.Equal("act", article.Execution(1).Action(1).Read().Label);
        Assert.Equal(2, article.Execution(1).Read().Times);
        //Assert.Equal([67, 14, 14, 67, 67, 14, 67, 67, 14, 14], collector);
    }
}