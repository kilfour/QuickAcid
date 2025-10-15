using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using QuickPulse.Bolts;
using StringExtensionCombinators;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.ObjectShrinking;

public class CustomPropertyShrinkingTests
{
    [Fact]
    public void For()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<Box<int>>.For(a => a.Value, a => [666])
            from input in "input".Input(Fuzz.Constant(new Box<int>(42)))
            from foo in "spec".Spec(() => { observe.Add(input.Value); return false; })
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.Contains(666, observe);
    }

    [Fact]
    public void None()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<Box<int>>.None(a => a.Value)
            from input in "input".Input(Fuzz.Constant(new Box<int>(42)))
            from foo in "spec".Spec(() => { observe.Add(input.Value); return false; })
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.All(observe, item => Assert.Equal(42, item));
    }
}
