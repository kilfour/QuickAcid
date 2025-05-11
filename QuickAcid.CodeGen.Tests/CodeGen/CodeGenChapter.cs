using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickAcid.TestsDeposition._Tools.Models;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen;

[Doc(Order = Order, Caption = "QuickAcid CodeGen", Content =
@"Show how to activate and retrieve the code and write it to file.

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
var script =
    from account in ""Account"".Tracked(
        () => new Account(), a => a.Balance.ToString())
    from _ in ""ops"".Choose(
        from depositAmount in ""deposit"".Input(MGen.Int(0, 10))
        from act in ""account.Deposit"".Act(
            () => account.Deposit(depositAmount))
        select Acid.Test,
        from withdrawAmount in ""withdraw"".Input(MGen.Int(42, 42))
        from withdraw in ""account.Withdraw:withdraw"".Act(
            () => account.Withdraw(withdrawAmount))
        select Acid.Test
    )
    from spec in ""No_Overdraft: account.Balance >= 0"".Spec(() => account.Balance >= 0)
    select Acid.Test;

var code = new QState(script).GenerateCode().Observe(50).Code;
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
")]
public class CodeGenChapter
{
    public const string Order = "1-90";

    [Fact]
    public void Example()
    {
        var script =
            from account in "Account".Tracked(() => new Account(), a => a.Balance.ToString())
            from _ in "ops".Choose(
                from depositAmount in "deposit".Input(MGen.Int(0, 10))
                from act in "account.Deposit".Act(() => account.Deposit(depositAmount))
                select Acid.Test,
                from withdrawAmount in "withdraw".Input(MGen.Int(42, 42))
                from withdraw in "account.Withdraw:withdraw".Act(() => account.Withdraw(withdrawAmount))
                select Acid.Test
            )
            from spec in "No_Overdraft: account.Balance >= 0".Spec(() => account.Balance >= 0)
            select Acid.Test;

        var code = new QCodeState(script).GenerateCode(50);

        var reader = LinesReader.FromText(code);
        Assert.Equal("namespace Refined.By.QuickAcid;", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal("public class UnitTests", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    [Fact]", reader.NextLine());
        Assert.Equal("    public void No_Overdraft()", reader.NextLine());
        Assert.Equal("    {", reader.NextLine());
        Assert.Equal("        var account = new Account();", reader.NextLine());
        Assert.Equal("        account.Withdraw(42);", reader.NextLine());
        Assert.Equal("        Assert.True(account.Balance >= 0);", reader.NextLine());
        Assert.Equal("    }", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.True(reader.EndOfCode());
    }
}