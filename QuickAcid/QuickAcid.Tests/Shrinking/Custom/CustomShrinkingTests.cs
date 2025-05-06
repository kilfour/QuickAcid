using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts.ShrinkStrats;
using QuickMGenerate;

namespace QuickAcid.Tests.Shrinking.Custom;

public class CustomShrinkingTests
{
    public class Shrinky : IShrinker<int>
    {
        public IEnumerable<int> Shrink(int value)
        {
            return [666];
        }
    }

    [Fact]
    public void Custom_using_class()
    {
        var observe = new HashSet<int>();
        var run =
            from _ in Shrink<int>.LikeThis(new Shrinky())
            from input in "input".Shrinkable(MGen.Constant(42))
            from foo in "spec".Spec(() => { observe.Add(input); return false; })
            select Acid.Test;
        var report = new QState(run).ObserveOnce();
        Assert.NotNull(report);
        Assert.Contains(666, observe);
    }

    [Fact]
    public void Custom_using_lambda()
    {
        var observe = new HashSet<int>();
        var run =
            from _ in Shrink<int>.LikeThis(a => [666])
            from input in "input".Shrinkable(MGen.Constant(42))
            from foo in "spec".Spec(() => { observe.Add(input); return false; })
            select Acid.Test;
        var report = new QState(run).ObserveOnce();
        Assert.NotNull(report);
        Assert.Contains(666, observe);
    }
}