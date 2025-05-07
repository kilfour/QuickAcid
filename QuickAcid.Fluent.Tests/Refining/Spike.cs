using QuickMGenerate;
using QuickAcid.Fluent;

namespace QuickAcid.Tests.Refining;

public class Account
{
    public int Balance = 0;
    public void Deposit(int amount) { Balance += amount; }
    public void Withdraw(int amount) { Balance -= amount; }
}

public class Spike
{
    [Fact(Skip = "demo")]
    public void FluentRefining()
    {
        SystemSpecs.Define()
            .Tracked("Account", () => new Account(), a => a.Balance.ToString())
            .Fuzzed("deposit", MGen.Int(0, 100))
            .Fuzzed("withdraw", MGen.Int(0, 100))
            .Options(opt =>
                [ opt.Do("account.Deposit:deposit", c => c.Account().Deposit(c.DepositAmount()))
                , opt.Do("account.Withdraw:withdraw", c => c.Account().Withdraw(c.WithdrawAmount()))
                ])
            .Assert("No Overdraft: account.Balance >= 0", c => c.Account().Balance >= 0)
            .Assert("Balance Has Maximum: account.Balance <= 100", c => c.Account().Balance <= 100)
            .DumpItInAcid();
        //.AndRunTheWohlwillProcess(50, 20);
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
