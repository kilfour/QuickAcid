using QuickPulse.Explains;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using QuickAcid.Tests._Tools;
using QuickAcid.Tests._Tools.ThePress;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.Collections;

public static class Chapter { public const string Order = "1-50-7"; }

[Doc(Order = Chapter.Order, Caption = "Collection Shrinking", Content =
@"...
")]
public class CollectionShrinkingTests
{
    [Fact]
    [Doc(Order = $"{Chapter.Order}-1", Content = @"Usage")]
    public void Collection_shrink()
    {
        var script =
            from input in "input".Input(Fuzz.Constant<IEnumerable<int>>([1, 2, 3]))
            from act in "act".Act(() => { })
            from spec in "spec".Spec(() => input.Count() <= 2)
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
                .WithOneRun()
                .And(15.ExecutionsPerRun()));

        Assert.Equal("[ _, _, _ ]", article.Execution(1).Input(1).Read().Value);
    }

    [Fact]
    public void Collection_nested_shrink()
    {
        var script =
            from input in "input".Input(Fuzz.Constant<IEnumerable<List<int>>>([[42]]))
            from act in "act".Act(() => { })
            from spec in "spec".SpecIf(
                () => input.Any() && input.First().Count != 0,
                () => input.First().First() != 42)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(15.ExecutionsPerRun()));

        var entry = article.Execution(1).Input(1).Read();
        Assert.Equal("[ [ 42 ] ]", entry.Value);
    }

    [Fact]
    public void Collection_shrink_with_extra()
    {
        var script =
            from input in "input".Input(Fuzz.Constant<IEnumerable<int>>([666, 42]))
            from act in "act".Act(() => { })
            from spec in "spec".SpecIf(() => input.Count() > 1, () => input.ElementAt(1) != 42)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(15.ExecutionsPerRun()));

        var entry = article.Execution(1).Input(1).Read();
        Assert.Equal("[ _, 42 ]", entry.Value);
    }

    public Generator<IEnumerable<int>> GrowingListUp()
    {
        var counter = 3;
        return
            state =>
                {
                    var list = new List<int>();
                    var i = 1;
                    counter.Times(() => list.Add(i++));
                    return new Result<IEnumerable<int>>(list, state);
                };
    }

    [Fact]
    public void Collection_shrink_with_dep()
    {
        var script =
            from input in "input".Input(Fuzz.Constant<IEnumerable<int>>([1, 2, 3]))
            from act in "act".Act(() => { })
            from spec in "spec".SpecIf(() => input.Count() > 2, () =>
            !(input.ToList()[0] == 1
            && input.ToList()[1] == 2
            && input.ToList()[2] == 3))
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(15.ExecutionsPerRun()));

        var entry = article.Execution(1).Input(1).Read();
        Assert.Equal("[ 1, 2, 3 ]", entry.Value);
    }

    [Fact]
    public void Collection_irrelevant_not_in_report()
    {
        var script =
            from input in "input".Input(Fuzz.Int().Many(3))
            from act in "act".Act(() => { })
            from spec in "spec".Spec(() => false)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.Equal(0, article.Total().Inputs());
    }

    [Fact]
    public void Collection_shrink_with_haha()
    {
        var script =
            from input in "input".Input(Fuzz.Constant<IEnumerable<int>>([1, 2, 1]))
            from act in "act".Act(() => { })
            from spec in "spec".SpecIf(() => input.Count() > 2, () => !input.Contains(1))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(15.ExecutionsPerRun()));

        var entry = article.Execution(1).Input(1).Read();
        Assert.Equal("[ _, _, 1 ]", entry.Value);
    }

    public class Composed
    {
        public int One { get; set; }
        public int Two { get; set; }
    }

    [Fact]
    public void Collection_shrink_with_hahaha()
    {
        var script =
            from input in "input".Input(Fuzz.Constant<IEnumerable<Composed>>(
                [new Composed() { One = 42, Two = 0 }, new Composed() { One = 42, Two = 0 }]))
            from act in "act".ActIf(() => input.Count() > 0, () => input.Any(a => a.One == 42))
            from spec in "spec".Spec(() => !act)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(15.ExecutionsPerRun()));

        Assert.Equal(1, article.Total().Inputs());
        var entry = article.Execution(1).Input(1).Read();
        Assert.Equal("[ { One : 42 } ]", entry.Value);
    }
}