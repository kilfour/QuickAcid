using QuickAcid.Tests._Tools.ThePress;
using QuickPulse.Arteries;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Bugs;

public class ExecutionShrinking
{
    [Fact]
    public void Choosing_Executions_Two_Need_To_Remain()
    {
        var script =
            from collector in "collector".Stashed(() => Collect.ValuesOf<int>())
            from ops in Script.Choose(
                "act1".Act(() => collector.Absorb(1)),
                "act2".Act(() => collector.Absorb(2)),
                "act3".Act(() => collector.Absorb(3))
            )
            from spec in "spec".Spec(() => collector.Values.Count == collector.Values.Distinct().Count())
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(30.ExecutionsPerRun()));

        Assert.Equal(2, article.Execution(1).Read().Times);
    }
}