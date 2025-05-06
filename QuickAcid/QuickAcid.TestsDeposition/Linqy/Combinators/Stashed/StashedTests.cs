using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickAcid.TestsDeposition._Tools.Models;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy.Combinators.Stashed;

public static class Chapter { public const string Order = "1-2-10"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Stashed", Content =
@"**Stashed(...)**
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
}

