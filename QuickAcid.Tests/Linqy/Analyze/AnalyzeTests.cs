using QuickAcid.Reporting;
using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;

namespace QuickAcid.Tests.Linqy.Analyze;

public class AnalyzeTests
{
    [Fact]
    public void Analyze_excutes_only_at_end_of_run()
    {
        var counter = 0;
        var script =
            from _a1 in "record".Act(() => counter++)
            from as1 in "spec".Analyze(() => false)
            select Acid.Test;

        TheJournalist.Exposes(() => QState.Run(script)
            .Options(a => a with { ShrinkingActions = true })
            .WithOneRun().And(2.ExecutionsPerRun()));

        var timesActShouldHaveRunOriginally = 2;
        var timesActShouldHaveRunDuringExcutionShrinking = 1;
        var timesActShouldHaveRunDuringInputShrinking = 0; // not run because ShrinkingActions removed it

        var timesRun =
            timesActShouldHaveRunOriginally
            + timesActShouldHaveRunDuringExcutionShrinking
            + timesActShouldHaveRunDuringInputShrinking;

        Assert.Equal(timesRun, counter);
    }

    [Fact]
    public void Analyze_as_assay()
    {
        var script =
            from observer in "observer".Tracked(() => new HashSet<int>())
            from roll in "roll".Act(() => Fuzz.Int(1, 3).Generate())
            from _a1 in "record".Act(() => observer.Add(roll))
            from as1 in "gens 3".Analyze(() => observer.Contains(3))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(20.ExecutionsPerRun()));


        Assert.Equal("gens 3", article.FailedSpec());

        // even though the test above will fails
        // and we expect all actions to be shrunk away, 
        // but because during shrinking the spec will not fail without an act
        // shrinking will leaves some behind.
        // for test like the one above Assay is a better candidate
        Assert.Equal(2, article.Total().Actions());
    }
}