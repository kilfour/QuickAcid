using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickAcid.TestsDeposition._Tools.Models;


namespace QuickAcid.TestsDeposition.Linqy.Combinators.Stashed;

public static class Chapter { public const string Order = "1-2-10"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Stashed", Content =
@"**Stashed(...)** lorum ipsum
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
        var run =
            from account in "account".Stashed(() => new Account())
            select Acid.Test;
        new QState(run).Testify(1);
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
        var run =
            from flag in "flag".StashedValue(true)
            select Acid.Test;
        new QState(run).Testify(1);
    }
}

