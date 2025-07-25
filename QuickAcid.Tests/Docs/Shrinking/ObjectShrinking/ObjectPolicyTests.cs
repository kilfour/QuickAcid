using QuickAcid.Tests._Tools.ThePress;
using QuickAcid.TestsDeposition._Tools;
using QuickFuzzr;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.ObjectShrinking;

public class ObjectPolicyTests
{
    [Fact]
    public void No_Strategies()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in ShrinkingPolicy.ForObjects()
            from input in "input".Input(Fuzz.Constant(new Container<int>(42)))
            from foo in "spec".Spec(() => { observe.Add(input.Value); return false; })
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.All(observe, item => Assert.Equal(42, item));
    }
}