using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts.ShrinkStrats;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;
using QuickMGenerate.UnderTheHood;
using QuickPulse.Arteries;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.Collections;

public static class Chapter { public const string Order = "1-50-7"; }

[Doc(Order = Chapter.Order, Caption = "Collection Shrinking", Content =
@"...
")]
public class CollectionShrinkingTests
{
    public Generator<IEnumerable<int>> GrowingList()
    {
        var counter = 0;
        return
            state =>
                {
                    return new Result<IEnumerable<int>>(Enumerable.Repeat(42, counter++).ToList(), state);
                };
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-1", Content = @"Usage")]
    public void Collection_shrink()
    {

        var script =
            from input in "input".Input(GrowingList())
            from act in "act".Act(() => { })
            from spec in "spec".Spec(() => input.Count() <= 2)
            select Acid.Test;
        var report = new QState(script).Observe(15);
        Assert.Null(report); // Input Shrinking is too greedy
    }
}