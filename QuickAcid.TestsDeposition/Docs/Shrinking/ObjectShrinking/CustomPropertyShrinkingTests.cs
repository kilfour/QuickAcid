using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.ObjectShrinking;

public class CustomPropertyShrinkingTests
{
    [Fact]
    public void For()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<Container<int>>.For(a => a.Value, a => [666])
            from input in "input".Input(MGen.Constant(new Container<int>(42)))
            from foo in "spec".Spec(() => { observe.Add(input.Value); return false; })
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);
        Assert.Contains(666, observe);
    }
}