using QuickMGenerate;
using QuickAcid.CodeGen;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Tests.CodeGen;

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
                from depositAmount in "deposit amount".Shrinkable(MGen.Int(0, 10))
                from act in "deposit".Act(() => account.Deposit(depositAmount))
                    .AddCode((key, store) => $"account.Deposit({store.Get<int>("deposit amount")})")
                select Acid.Test,
                from withdrawAmount in "withdraw amount".Shrinkable(MGen.Int(42, 42))
                from withdraw in "withdraw".Act(() => account.Withdraw(withdrawAmount))
                    .AddCode((key, store) => $"account.Withdraw({store.Get<int>("withdraw amount")})")
                select Acid.Test
            )
            from spec in "No Overdraft".Spec(() => account.Balance >= 0).AddCode((key, store) => "account.Balance >= 0")
            select Acid.Test;

        var reader = LinesReader.FromFailingRunTryFiftyTimes(run);
        Assert.Equal("[Fact]", reader.NextLine());
        Assert.Equal("public void No_Overdraft()", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    var account = new Account();", reader.NextLine());
        Assert.Equal("    account.Withdraw(42);", reader.NextLine());
        Assert.Equal("    Assert.True(account.Balance >= 0);", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfCode());
    }

    [Fact]
    public void Without_code_decorators()
    {
        var run =
            from account in "Account".AlwaysReported(() => new Account(), a => a.Balance.ToString())
            from _ in "ops".Choose(
                from depositAmount in "deposit".Shrinkable(MGen.Int(0, 10))
                from act in "account.Deposit".Act(() => account.Deposit(depositAmount))
                select Acid.Test,
                from withdrawAmount in "withdraw".Shrinkable(MGen.Int(42, 42))
                from withdraw in "account.Withdraw:withdraw".Act(() => account.Withdraw(withdrawAmount))
                select Acid.Test
            )
            from spec in "No_Overdraft: account.Balance >= 0".Spec(() => account.Balance >= 0)
            select Acid.Test;

        QAcidDebug.Write(run.ReportIfFailed(1, 50).ToString());
        QAcidDebug.WriteLine("-- Ladies and Gentleman, ..., and now, ...");
        QAcidDebug.WriteLine("");
        QAcidDebug.WriteLine(run.ToCodeIfFailed(1, 50).ToString());


        var reader = LinesReader.FromFailingRunTryFiftyTimes(run);
        Assert.Equal("[Fact]", reader.NextLine());
        Assert.Equal("public void No_Overdraft()", reader.NextLine());
        Assert.Equal("{", reader.NextLine());
        Assert.Equal("    var account = new Account();", reader.NextLine());
        Assert.Equal("    account.Withdraw(42);", reader.NextLine());
        Assert.Equal("    Assert.True(account.Balance >= 0);", reader.NextLine());
        Assert.Equal("}", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfCode());
    }
}
