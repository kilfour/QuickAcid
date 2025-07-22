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
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .AndOneExecutionPerRun();
        Assert.NotNull(report);
        Assert.All(observe, item => Assert.Equal(42, item));
    }
}