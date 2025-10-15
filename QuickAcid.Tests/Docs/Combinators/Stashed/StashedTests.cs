using QuickAcid.Tests._Tools.Models;
using QuickPulse.Explains;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Docs.Combinators.Stashed;

[DocFile]
[DocContent(
@"**Stashed(...)** creates a value once at the start of the test run and reuses it across all executions.  
This is typically where you'd stash your **system under test (SUT)** — a service, container, or domain object whose behavior you're exploring.  
Unlike `Input(...)`, stashed values are fixed for the entire run and never shrink, making them ideal for holding mutable state or orchestrating effects across inputs.
")]
public class StashedTests
{
    [Fact]
    [DocContent(
@"**Usage example:**
```csharp
from account in ""account"".Stashed(() => new Account())
```
")]
    public void Stashed_usage()
    {
        var script =
            from account in "account".Stashed(() => new Account())
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
    }

    [Fact]
    [DocContent(
@"Stashes a primitive or scalar value without requiring a wrapper object.
Intended for flags, counters, and other small mutable state used during generation.  

**Example:**
```csharp
from flag in ""flag"".StashedValue(true)
```
")]
    public void StashedValue_usage()
    {
        var script =
            from flag in "flag".StashedValue(true)
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
    }
}

