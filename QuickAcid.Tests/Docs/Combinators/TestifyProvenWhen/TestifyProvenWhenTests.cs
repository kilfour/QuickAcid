using QuickAcid.TestsDeposition._Tools;
using QuickPulse.Explains;
using QuickFuzzr;
using QuickAcid.Tests._Tools.ThePress;

namespace QuickAcid.TestsDeposition.Docs.Combinators.TestifyProvenWhen;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-200"; }


[Doc(Order = $"{Chapter.Order}", Caption = "TestifyProvenWhen", Content =
@"**TestifyProvenWhen(...)**
Ends the test run early once a specified condition is satisfied.
This combinator is not a property specification itself,
but a control structure that governs when a test run is considered 'proven' and can terminate before reaching the maximum number of executions. It's typically used in combination with `Stashed(...)` or other state-tracking steps that accumulate evidence across runs.
")]
public class TestifyProvenWhenTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from seenTrue in ""val is true"".TestifyProvenWhen(() => container.Value)
```
")]
    [Fact]
    public void TestifyProvenWhen_usage()
    {
        var script =
            from container in "container".Stashed(() => new Container<bool>(false))
            from val in "bool".Derived(Fuzz.Constant(true))
            from act in "act".Act(() => container.Value = container.Value | val)
            from spec in "val is true".TestifyProvenWhen(() => container.Value)
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
    }

    [Doc(Order = $"{Chapter.Order}-2", Content =
@"
This would end the test run early once `container.Value` becomes `true`.
")]
    [Fact]
    public void TestifyProvenWhen_breaks_run_early()
    {
        var counter = 0;
        var script =
            from act in "act".Act(() => counter++)
            from spec in "val is true".TestifyProvenWhen(() => counter == 3)
            select Acid.Test;
        QState.Run(script).WithOneRun().And(100.ExecutionsPerRun());
        Assert.Equal(3, counter);
    }

    [Doc(Order = $"{Chapter.Order}-3", Content =
@"**Note:** This does not assert a property directly — use `Assay(...)` or `Analyze(...)` for that.
`TestifyProvenWhen(...)` is about controlling *how long* a test runs based on dynamic conditions observed during execution.
")]
    [Fact]
    public void TestifyProvenWhen_scaffold()
    {
        var script =
            from container in "container".Stashed(() => new Container<bool>(false))
            from val in "bool".Derived(Fuzz.Constant(false))
            from act in "act".Act(() => container.Value = container.Value | val)
            from spec in "early exit".TestifyProvenWhen(() => container.Value)
            from finalspec in "val is true".Assay(() => container.Value)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(100.ExecutionsPerRun()));
        Assert.Equal("val is true", article.AssayerDisagrees());
    }
}

