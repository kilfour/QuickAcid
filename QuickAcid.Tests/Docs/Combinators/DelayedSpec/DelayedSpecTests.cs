using QuickAcid.Reporting;
using QuickAcid.Tests._Tools.ThePress;
using QuickPulse.Explains;


namespace QuickAcid.TestsDeposition.Docs.Combinators.DelayedSpec;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-67"; }

[Doc(Order = $"{Chapter.Order}", Caption = "DelayedSpec", Content =
@"**DelayedSpec(...)** ... TODO ...
")]
public class DelayedSpecTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from spec in ""spec"".DelayedSpec(() => false)
from trace in ""trace"".TraceIf(() => spec.Failed, spec.Label)
let apply = spec.Apply()
```
")]
    [Fact]
    public void DelayedSpec_usage_failed_not_applied()
    {

        var script =
            from _ in "act1".Act(() => { })
            from specResult in "spec".DelayedSpec(() => false)
            select Acid.Test;
        var article = TheJournalist.Unearths(QState.Run(script).WithOneRunAndOneExecution());
        Assert.False(article.VerdictReached());
    }

    [Fact]
    public void DelayedSpec_usage_failed_applied()
    {
        var script =
            from _ in "act1".Act(() => { })
            from spec in "spec".DelayedSpec(() => false)
            let apply = spec.Apply()
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script).WithOneRunAndOneExecution());
        Assert.Equal("spec", article.FailedSpec());
    }
}

