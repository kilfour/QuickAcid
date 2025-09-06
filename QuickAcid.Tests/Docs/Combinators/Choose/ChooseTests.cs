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
        var script =
            from signal in Script.Stashed(() => new HashSet<int>())
            from _ in Script.Choose(
                "one".Act(() => signal.Add(1)),
                "two".Act(() => signal.Add(2))
                )
            from __ in "all actions hit".TestifyProvenWhen(() => signal.Count == 2)
            select Acid.Test;
        QState.Run(script)
            .WithOneRun()
            .And(50.ExecutionsPerRun());
    }
}

