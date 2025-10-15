using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Bugs;

public class SpecIfNotEvaluated
{
    [Fact]
    public void Try_A_Number()
    {
        var script =
            from input in "input".Input(Fuzz.Constant(42))
            from act1 in "act1".Act(() => { return 5 * input; })
            from act2 in "act2".Act(() => { return 6 * input; })
            from spec in "spec".SpecIf(() => act1 != 0, () => act1 == act2)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(30.ExecutionsPerRun()));

        Assert.Equal(1, article.Total().Inputs());
    }
}