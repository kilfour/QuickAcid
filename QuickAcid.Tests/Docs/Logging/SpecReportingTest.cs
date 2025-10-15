using QuickAcid.Tests._Tools.ThePress;
using StringExtensionCombinators;

namespace QuickAcid.TestsDeposition.Docs.Logging;

public class SpecReportingTest
{
    [Fact] // needs test that this only registers original run counts
    public void Count()
    {
        var counter = 0;
        var script =
            from act in "act".Act(() => counter++)
            from s1 in "Spec One".Spec(() => true)
            from s2 in "Spec Two".SpecIf(() => counter > 2, () => counter < 4)
            from s3 in "Spec Three".Spec(() => true)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(10.ExecutionsPerRun()));

        var passedSpec = article.PassedSpec(1).Read();
        Assert.Equal("Spec One", passedSpec.Label);
        Assert.Equal(4, passedSpec.TimesPassed);

        passedSpec = article.PassedSpec(2).Read();
        Assert.Equal("Spec Three", passedSpec.Label);
        Assert.Equal(3, passedSpec.TimesPassed);

        passedSpec = article.PassedSpec(3).Read();
        Assert.Equal("Spec Two", passedSpec.Label);
        Assert.Equal(1, passedSpec.TimesPassed);
    }
}