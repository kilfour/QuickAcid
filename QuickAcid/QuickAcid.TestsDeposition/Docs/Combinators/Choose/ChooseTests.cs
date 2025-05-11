using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickAcid.TestsDeposition._Tools.Models;
using QuickMGenerate;
using QuickPulse;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Choose;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-70"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Choose", Content =
@"**Choose(...)** is one of two ways one can select different operations per execution.
In the case of choose, you supply a number of `.Act(...)`'s and QuickAcid will randomly choose one for every execution. 
")]
public class ChooseTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from _ in ""ops"".Choose(
    ""deposit"".Act(() => account.Deposit(500)),
    ""withdraw"".Act(() => account.Withdraw(500))
    )
```
")]
    [Fact]
    public void Choose_usage()
    {
        // var hashset = new HashSet<string>();
        // var run =
        //     from account in "Account".Stashed(() => new Account())
        //     from _ in "ops".Choose(
        //         "deposit".Act(() => signal.pulse),
        //         "withdraw".Act(() => account.Withdraw(500))
        //         )
        //     from testify in "".TestifyProvenWhen()
        //     from spec in "fail".Spec(() => false)
        //     select Acid.Test;
        // var report = new QState(run).Observe(2);
        // var entry = report.First<ReportExecutionEntry>();
        // Assert.NotNull(entry);
        // Assert.Equal("act", entry.Key);
    }
}

