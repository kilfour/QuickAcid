using QuickPulse.Explains;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using QuickAcid.Tests._Tools.ThePress;
using StringExtensionCombinators;

namespace QuickAcid.TestsDeposition.Docs.Combinators.Trace;

[DocFile]
[DocContent(
@"**Trace(...)** Used to add information from the script to the QuickAcid report.
")]
public class TraceTests
{
    [Fact]
    [DocContent(
@"**Usage example:**
```csharp
from _ in ""Info Key"".Trace(() => ""Extra words"")
```
")]
    public void Trace_usage()
    {
        var script =
            from _ in "act".Act(() => { })
            from __ in "Info Key".Trace(() => "Extra words")
            select Acid.Test;

        var article = TheJournalist.Unearths(
            QState.Run(script)
            .WithOneRun()
            .And(5.ExecutionsPerRun()));

        // No Verdict => No Trace
        // Assert.Equal("Extra words", article.Execution(1).Trace(1).Read().Value);
    }

    [Fact]
    public void Trace_with_shrinking()
    {
        var counter = 0;
        var script =
            from ints in "ints".Input(Fuzzr.Constant(42).Many(2))
            from _ in "act".Act(() => { counter++; })
            from __ in "Info Key".Trace(() => $"[ {string.Join(", ", ints)} ]")
            from ___ in "spec".Spec(() => counter < 3)
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
                .WithOneRun()
                .And(5.ExecutionsPerRun()));

        Assert.Equal("[ 42, 42 ]", article.Execution(1).Trace(1).Read().Value);
    }
    [Fact]
    public void Trace_with_other_shrinking()
    {
        var script =
            from input in "input".Input(Fuzzr.Counter("id"))
            from list in "list".Stashed(() => new List<int>())
            from _ in "act".Act(() => { })
            from delayedSpec in "spec".DelayedSpec(() => input != 3)
            from __ in "Info Key".TraceIf(() => delayedSpec.Failed, () => $"{input}")
            let ___ = delayedSpec.Apply()
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(5.ExecutionsPerRun()));

        Assert.Equal("3", article.Execution(1).Trace(1).Read().Value);
    }

    [Fact]
    [DocContent(
@"**TraceIf(...)** is the same as `Trace(...)` but only injects information in the report conditionally.  

**Usage example:**
```csharp
from _ in ""Info Key"".TraceIf(() => number == 42, () => ""Extra words"")
```
")]
    public void TraceIf_usage()
    {
        var script =
            from input in "input".Input(Fuzzr.Constant(42))
            from _ in "act".Act(() => { })
            from __ in "Trace 42".TraceIf(() => input == 42, () => "YEP 1")
            from ___ in "Trace not 42".TraceIf(() => input != 42, () => "YEP 2")
            from ____ in "spec".Spec(() => false) // <= Needs to fail for Traces to show up
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script).WithOneRunAndOneExecution());
        var entry = article.Execution(1).Trace(1).Read();
        Assert.Equal("Trace 42", entry.Label);
        Assert.Equal("YEP 1", entry.Value);
    }
}

