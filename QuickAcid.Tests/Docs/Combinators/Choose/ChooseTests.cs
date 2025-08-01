using QuickPulse.Explains;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Choose;

[DocFile]
[DocContent(
@"**Choose(...)** is one of two ways one can select different operations per execution.
In the case of choose, you supply a number of `.Act(...)`'s and QuickAcid will randomly choose one for every execution. 
")]
public class ChooseTests
{
    [Fact]
    [DocContent(
@"**Usage example:**
```csharp
from _ in ""ops"".Choose(
    ""deposit"".Act(() => account.Deposit(500)),
    ""withdraw"".Act(() => account.Withdraw(500))
    )
```
")]
    public void Choose_usage()
    {
        // var hashset = new HashSet<string>();
        // var script =
        //     from account in "Account".Stashed(() => new Account())
        //     from _ in "ops".Choose(
        //         "deposit".Act(() => signal.pulse),
        //         "withdraw".Act(() => account.Withdraw(500))
        //         )
        //     from testify in "".TestifyProvenWhen()
        //     from spec in "fail".Spec(() => false)
        //     select Acid.Test;
        // var report = QState.Run(script)
        // .Options(a => a with { DontThrow = true })
        // .WithOneRun()
        // .And(2);
        // var entry = report.First<ReportExecutionEntry>();
        // Assert.NotNull(entry);
        // Assert.Equal("act", entry.Key);
    }
}

