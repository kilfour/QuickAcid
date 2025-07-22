using QuickAcid.Reporting;
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
        var counter = 0;
        var script =
            from _ in "act1".Act(() => { counter++; })
            from specResult in "spec".DelayedSpec(() => counter < 3)
            select Acid.Test;
        Assert.True(QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .And(5.ExecutionsPerRun()).IsSuccess);
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
        var report = QState.Run(script)
            .Options(a => a with { DontThrow = true })
            .WithOneRun()
            .And(5.ExecutionsPerRun());

        var entry = report.Single<ReportTraceEntry>();
        Assert.Equal("spec", entry.Value);
    }
}

