using QuickAcid.TestsDeposition._Tools.Models;
using QuickPulse.Explains;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Stashed;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-10"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Stashed", Content =
@"**Stashed(...)** creates a value once at the start of the test run and reuses it across all executions.  
This is typically where you'd stash your **system under test (SUT)** — a service, container, or domain object whose behavior you're exploring.  
Unlike `Input(...)`, stashed values are fixed for the entire run and never shrink, making them ideal for holding mutable state or orchestrating effects across inputs.
")]
public class StashedTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from account in ""account"".Stashed(() => new Account())
```
")]
    [Fact]
    public void Stashed_usage()
    {
        var script =
            from account in "account".Stashed(() => new Account())
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
    }
    [Doc(Order = $"{Chapter.Order}-2-1", Caption = "StashedValue", Content =
@"Stashes a primitive or scalar value without requiring a wrapper object.
Intended for flags, counters, and other small mutable state used during generation.  

**Example:**
```csharp
from flag in ""flag"".StashedValue(true)
```
")]
    [Fact]
    public void StashedValue_usage()
    {
        var script =
            from flag in "flag".StashedValue(true)
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
    }
}

