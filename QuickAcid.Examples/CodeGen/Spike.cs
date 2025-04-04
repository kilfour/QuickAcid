using QuickMGenerate;

namespace QuickAcid.Examples.CodeGen;

public static class Ext
{
    public static QAcidRunner<T> AddCode<T>(this QAcidRunner<T> runner, Func<string, Memory.Access, string> toCode)
    {
        return
            s =>
            {
                return runner(s);
            };
    }
}

public class Spike : QAcidLoggingFixture
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
            from account in "account".TrackedInput(() => new Account())
                .AddCode((key, store) => $"var {key} = new Account();")
            from _ in "ops".Choose(
                from depositAmount in "deposit amount".ShrinkableInput(MGen.Int(0, 10))
                from act in "deposit".Act(() => account.Deposit(depositAmount))
                    .AddCode((key, store) => $"account.Deposit({store.Get<int>("deposit amount")});")
                select Acid.Test,
                from withdrawAmount in "withdraw amount".ShrinkableInput(MGen.Int(0, 10))
                    .AddCode((key, store) => $"account.Withdraw({store.Get<int>("withdraw amount")});")
                from withdraw in "withdraw".Act(() => account.Withdraw(withdrawAmount))
                select Acid.Test
            )
            from spec in "No Overdraft".Spec(() => account.Balance >= 0).AddCode((key, store) => "account.Balance >= 0")
            select Acid.Test;


        // var report = run.ReportIfFailed(1, 50);
        // if (report != null)
        //     QAcidDebug.Write(report.ToString());

        var code = run.ToCodeIfFailed(1, 50);
        // if (code != null)
        //     QAcidDebug.Write(code);

        var expected =
@"[Fact]
public void No_Overdraft()
{
    var account = new Account();
    account.Withdraw(5);
    Assert.True(account.Balance >= 0);
}
";
        Assert.Equal(code, expected);
    }
}