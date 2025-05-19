using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;


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
        var counter = 0;
        var script =
            from _ in "act1".Act(() => { counter++; })
            from specResult in "spec".DelayedSpec(() => counter < 3)
            select Acid.Test;
        Assert.Null(new QState(script).Observe(5));
    }

    [Fact]
    public void DelayedSpec_usage_failed_applied()
    {
        var counter = 0;
        var script =
            from _ in "act1".Act(() => { counter++; })
            from spec in "spec".DelayedSpec(() => counter < 3)
            from trace in "trace".TraceIf(() => spec.Failed, spec.Label)
            let apply = spec.Apply()
            select Acid.Test;
        var report = new QState(script).Observe(5);
        Assert.NotNull(report);
        var entry = report.Single<ReportTraceEntry>();
        Assert.Equal("spec", entry.Value);
    }
}

