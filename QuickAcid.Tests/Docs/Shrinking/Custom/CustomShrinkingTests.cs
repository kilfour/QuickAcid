using QuickFuzzr;
using QuickAcid.Tests._Tools.ThePress;
using QuickAcid.Shrinking.Collections;
using QuickAcid.Shrinking.Custom;
using StringExtensionCombinators;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.Custom;

public static class Chapter { public const string Order = "1-50-20"; }


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
        var script =
            from _ in Shrink<int>.LikeThis(new Shrinky())
            from input in "input".Input(Fuzz.Constant(42))
            from foo in "spec".Spec(() => { observe.Add(input); return false; })
            select Acid.Test;

        TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.Contains(666, observe);
    }

    [Fact]
    public void Custom_using_lambda()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<int>.LikeThis(a => [666])
            from input in "input".Input(Fuzz.Constant(42))
            from foo in "spec".Spec(() => { observe.Add(input); return false; })
            select Acid.Test;
        TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.Contains(666, observe);
    }

    [Fact]
    public void Custom_Does_actually_shrink()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<int>.LikeThis(a => [666])
            from input in "input".Input(Fuzz.Constant(42))
            from foo in "spec".Spec(() => input != 42)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var entry = article.Execution(1).Input(1).Read();
        Assert.Equal("42", entry.Value);
    }

    [Fact]
    public void Custom_using_list()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<IEnumerable<int>>.LikeThis(a => [[666]])
            from input in "input".Input(Fuzz.Constant(42).Many(1))
            from foo in "spec".Spec(() => { input.ForEach(a => observe.Add(a)); return false; })
            select Acid.Test;
        TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.Contains(666, observe);
    }

    [Fact]
    public void Custom_collection_shrinking_policies_RemoveOneByOneStrategy()
    {
        var script =
            from _ in ShrinkingPolicy.ForCollections(new RemoveOneByOneStrategy())
            from input in "input".Input(Fuzz.Constant<List<int>>([42, 1, 2]))
            from foo in "spec".Spec(() => !input.Contains(42))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var entry = article.Execution(1).Input(1).Read();
        Assert.Equal("[ 42 ]", entry.Value);
    }

    [Fact]
    public void Custom_collection_shrinking_policies_ShrinkEachElementStrategy()
    {
        var script =
            from _ in ShrinkingPolicy.ForCollections(new ShrinkEachElementStrategy())
            from input in "input".Input(Fuzz.Constant<List<int>>([42, 1, 2]))
            from foo in "spec".Spec(() => !input.Contains(42))
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var entry = article.Execution(1).Input(1).Read();
        Assert.Equal("[ 42, _, _ ]", entry.Value);
    }

    [Fact]
    public void None()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<int>.None()
            from input in "input".Input(Fuzz.Constant(42))
            from foo in "spec".Spec(() => { observe.Add(input); return false; })
            select Acid.Test;

        TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        Assert.All(observe, item => Assert.Equal(42, item));
    }
}