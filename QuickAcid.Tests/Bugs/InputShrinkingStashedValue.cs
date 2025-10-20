using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using QuickPulse.Arteries;
using QuickPulse.Bolts;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Bugs;

public class InputShrinkingStashedValue
{
    [Fact]
    public void Looking()
    {
        var script =
            from storage in "storage".Stashed(() => TheCollector.Exhibits<int>())
            from count in Script.Stashed(() => new Box<int>(0))
            from inc in Script.Execute(() => count.Value++)
            from _ in Script.ChooseIf(
                (() => count.Value == 1,
                    from input in "input a".Input(Fuzzr.Constant(42))
                    from a in "a".Act(() => { storage.Absorb(input); })
                    select Acid.Test),
                (() => storage.TheExhibit.Count != 0,
                    from input in "input b".Input(Fuzzr.Constant(42))
                    from a in "b".Act(() => { })
                    from spec in "spec b".Spec(() => storage.TheExhibit[0] != input)
                    select Acid.Test))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(2.ExecutionsPerRun()));

        var inputA = article.Execution(1).Input(1).Read();
        Assert.Equal("input a", inputA.Label);
        Assert.Equal("42", inputA.Value);

        var inputB = article.Execution(2).Input(1).Read();
        Assert.Equal("input b", inputB.Label);
        Assert.Equal("42", inputB.Value);
    }
}