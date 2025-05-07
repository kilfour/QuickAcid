## QuickAcid CodeGen

Show how to activate and retrieve the code and write it to file.

Model used in all/most examples (until we need a more complex one) :
```csharp
public class Account
{
    public int Balance = 0;
    public void Deposit(int amount) { Balance += amount; }
    public void Withdraw(int amount) { Balance -= amount; }
}
```
TestExample:
```csharp
var run =
    from account in "Account".Tracked(
        () => new Account(), a => a.Balance.ToString())
    from _ in "ops".Choose(
        from depositAmount in "deposit".Input(MGen.Int(0, 10))
        from act in "account.Deposit".Act(
            () => account.Deposit(depositAmount))
        select Acid.Test,
        from withdrawAmount in "withdraw".Input(MGen.Int(42, 42))
        from withdraw in "account.Withdraw:withdraw".Act(
            () => account.Withdraw(withdrawAmount))
        select Acid.Test
    )
    from spec in "No_Overdraft: account.Balance >= 0".Spec(() => account.Balance >= 0)
    select Acid.Test;

var code = new QState(run).GenerateCode().Observe(50).Code;
```
Result:
```csharp
namespace Refined.By.QuickAcid;

public class UnitTests
{
    [Fact]
    public void No_Overdraft()
    {
        var account = new Account();
        account.Withdraw(42);
        Assert.True(account.Balance >= 0);
    }
}
```



