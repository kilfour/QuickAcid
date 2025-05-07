using QuickMGenerate;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;
using QuickPulse.Diagnostics.Sinks.FileWriters;

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
    public void Example()
    {
        var run =
            from account in "Account".Tracked(() => new Account(), a => a.Balance.ToString())
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
        // QAcidDebug.Write(report.ToString());
        // QAcidDebug.WriteLine("-- Ladies and Gentleman, ..., and now, ...");
        // QAcidDebug.WriteLine("");
        // QAcidDebug.WriteLine(run.ToCodeIfFailed(1, 50).ToString());


        // var reader = LinesReader.FromFailingRunTryFiftyTimes(run);
        // Assert.Equal("[Fact]", reader.NextLine());
        // Assert.Equal("public void No_Overdraft()", reader.NextLine());
        // Assert.Equal("{", reader.NextLine());
        // Assert.Equal("    var account = new Account();", reader.NextLine());
        // Assert.Equal("    account.Withdraw(42);", reader.NextLine());
        // Assert.Equal("    Assert.True(account.Balance >= 0);", reader.NextLine());
        // Assert.Equal("}", reader.NextLine());
        // Assert.Equal("", reader.NextLine());
        // Assert.True(reader.EndOfCode());
    }
}
