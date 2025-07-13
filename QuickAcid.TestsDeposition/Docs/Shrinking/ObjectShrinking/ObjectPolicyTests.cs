using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.ObjectShrinking;

public class ObjectPolicyTests
{
    [Fact]
    public void No_Strategies()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in ShrinkingPolicy.ForObjects()
            from input in "input".Input(MGen.Constant(new Container<int>(42)))
            from foo in "spec".Spec(() => { observe.Add(input.Value); return false; })
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);
        Assert.All(observe, item => Assert.Equal(42, item));
    }
}