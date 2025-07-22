using QuickAcid.TestsDeposition._Tools;
using QuickFuzzr;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.ObjectShrinking;

public class CustomPropertyShrinkingTests
{
    [Fact]
    public void For()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<Container<int>>.For(a => a.Value, a => [666])
            from input in "input".Input(Fuzz.Constant(new Container<int>(42)))
            from foo in "spec".Spec(() => { observe.Add(input.Value); return false; })
            select Acid.Test;
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();
        Assert.NotNull(report);
        Assert.Contains(666, observe);
    }

    [Fact]
    public void None()
    {
        var observe = new HashSet<int>();
        var script =
            from _ in Shrink<Container<int>>.None(a => a.Value)
            from input in "input".Input(Fuzz.Constant(new Container<int>(42)))
            from foo in "spec".Spec(() => { observe.Add(input.Value); return false; })
            select Acid.Test;
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();
        Assert.NotNull(report);
        Assert.All(observe, item => Assert.Equal(42, item));
    }
}
