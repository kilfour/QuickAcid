using QuickMGenerate;
using QuickAcid.CodeGen;
using QuickAcid.Nuts;
using QuickAcid.Nuts.Bolts;

namespace QuickAcid.Examples.CodeGen;

public class Spike
{
    public class Account
    {
        public int Balance = 0;
        public void Deposit(int amount) { Balance += amount; }
        public void Withdraw(int amount) { Balance -= amount; }
    }

    [Fact]
    public void InitialTry()
    {
        var run =
            from account in "account".AlwaysReported(() => new Account())
                .AddCode((key, store) => $"var {key} = new Account();")
            from _ in "ops".Choose(
                from depositAmount in "deposit amount".ShrinkableInput(MGen.Int(0, 10))
                from act in "deposit".Act(() => account.Deposit(depositAmount))
                    .AddCode((key, store) => $"account.Deposit({store.Get<int>("deposit amount")})")
                select Acid.Test,
                from withdrawAmount in "withdraw amount".ShrinkableInput(MGen.Int(42, 42))
                from withdraw in "withdraw".Act(() => account.Withdraw(withdrawAmount))
                    .AddCode((key, store) => $"account.Withdraw({store.Get<int>("withdraw amount")})")
                select Acid.Test
            )
            from spec in "No Overdraft".Spec(() => account.Balance >= 0).AddCode((key, store) => "account.Balance >= 0")
            select Acid.Test;

        var code = run.ToCodeIfFailed(1, 50);
        Assert.NotNull(code);

        var lines = code.Split(Environment.NewLine);
        Assert.Equal("[Fact]", lines[0]);
        Assert.Equal("public void No_Overdraft()", lines[1]);
        Assert.Equal("{", lines[2]);
        Assert.Equal("    var account = new Account();", lines[3]);
        Assert.Equal("    account.Withdraw(42);", lines[4]);
        Assert.Equal("    Assert.True(account.Balance >= 0);", lines[5]);
        Assert.Equal("}", lines[6]);
        Assert.Equal("", lines[7]);
    }
}