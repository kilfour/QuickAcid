using QuickMGenerate;
using QuickAcid.CodeGen;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Bolts;

namespace QuickAcid.Tests.Refining;

public class Account
{
    public int Balance = 0;
    public void Deposit(int amount) { Balance += amount; }
    public void Withdraw(int amount) { Balance -= amount; }
}

public class Spike
{


    [Fact]
    public void Lets_see_where_this_vein_leads()
    {
        var run =
            from account in "Account".AlwaysReported(() => new Account(), a => a.Balance.ToString())
            from _ in "ops".Choose(
                from depositAmount in "deposit".Shrinkable(MGen.Int(0, 100))
                from act in "account.Deposit:deposit".Act(() => account.Deposit(depositAmount))
                select Acid.Test,
                from withdrawAmount in "withdraw".Shrinkable(MGen.Int(0, 100))
                from withdraw in "account.Withdraw:withdraw".Act(() => account.Withdraw(withdrawAmount))
                select Acid.Test
            )
            from _s1 in "No Overdraft: account.Balance >= 0".Spec(() => account.Balance >= 0)
            from _s2 in "Balance Has Maximum: account.Balance <= 100".Spec(() => account.Balance <= 100)
            select Acid.Test;

        run.TheWohlwillProcess(20, 20);
    }

    [Fact]
    public void FluentRefining()
    {
        SystemSpecs.Define()
            .AlwaysReported("Account", () => new Account(), a => a.Balance.ToString())
            .Fuzzed("deposit", MGen.Int(0, 100))
            .Fuzzed("withdraw", MGen.Int(0, 100))
            .Options(opt =>
                [ opt.Do("account.Deposit:deposit", c => c.Account().Deposit(c.DepositAmount()))
                , opt.Do("account.Withdraw:withdraw", c => c.Account().Withdraw(c.WithdrawAmount()))
                ])
            .Assert("No Overdraft: account.Balance >= 0", c => c.Account().Balance >= 0)
            .Assert("Balance Has Maximum: account.Balance <= 100", c => c.Account().Balance <= 100)
            .DumpItInAcid()
            .AndRunTheWohlwillProcess(50, 20);
    }
}

public static class ContextExtensions
{
    public static Account Account(this QAcidContext context)
        => context.GetItAtYourOwnRisk<Account>("Account");
    public static int DepositAmount(this QAcidContext context)
        => context.GetItAtYourOwnRisk<int>("deposit");
    public static int WithdrawAmount(this QAcidContext context)
        => context.GetItAtYourOwnRisk<int>("withdraw");
}
