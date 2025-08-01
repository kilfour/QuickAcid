using QuickAcid.Bolts;
using QuickAcid.Tests._Tools.ThePress;
using QuickPulse.Explains;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Spec;

[DocFile]
[DocContent(
@"**Spec(...)** ... TODO ...
")]
public class SpecTests
{
    [Fact]
    [DocContent(
@"**Usage example:**
```csharp
from specResult in ""spec"".Spec(() => false)
```
")]
    public void Spec_usage()
    {
        var script =
            from _ in "act1".Act(() => { })
            from specResult in "spec".Spec(() => false)
            select Acid.Test;
        Assert.Throws<FalsifiableException>(() => QState.Run(script).WithOneRunAndOneExecution());
    }

    [Fact]
    public void DelayedSpec_usage_failed_not_applied()
    {
        var counter = 0;
        var script =
            from _ in "act1".Act(() => { counter++; })
            from specResult in "spec".DelayedSpec(() => counter < 3)
            select Acid.Test;
        QState.Run(script).WithOneRun().And(5.ExecutionsPerRun());
    }

    [Fact]
    public void DelayedSpec_usage_failed_applied()
    {
        var counter = 0;
        var script =
            from _ in "act1".Act(() => { counter++; })
            from spec in "spec".DelayedSpec(() => counter < 3)
            from trace in "trace".TraceIf(() => spec.Failed, () => spec.Label)
            let apply = spec.Apply()
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script).WithOneRun().And(5.ExecutionsPerRun()));

        Assert.Equal("spec", article.Execution(1).Trace(1).Read().Value);
    }
}

