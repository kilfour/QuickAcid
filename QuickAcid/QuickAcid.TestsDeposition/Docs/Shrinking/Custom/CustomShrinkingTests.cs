using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.Custom;

public static class Chapter { public const string Order = "1-50-20"; }

[Doc(Order = Chapter.Order, Caption = "Custom Shrinking", Content =
@"...
")]
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
    [Doc(Order = $"{Chapter.Order}-1", Content = @"Usage with class")]
    public void Custom_using_class()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<int>.LikeThis(new Shrinky())
            from input in "input".Input(MGen.Constant(42))
            from foo in "spec".Spec(() => { observe.Add(input); return false; })
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);
        Assert.Contains(666, observe);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-2", Content = @"Usage with lambda")]
    public void Custom_using_lambda()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<int>.LikeThis(a => [666])
            from input in "input".Input(MGen.Constant(42))
            from foo in "spec".Spec(() => { observe.Add(input); return false; })
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);
        Assert.Contains(666, observe);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-3", Content = @"custom list strat")]
    public void Custom_using_list()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<IEnumerable<int>>.LikeThis(a => [[666]])
            from input in "input".Input(MGen.Constant(42).Many(1))
            from foo in "spec".Spec(() => { input.ForEach(a => observe.Add(a)); return false; })
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);
        Assert.Contains(666, observe);
    }
}