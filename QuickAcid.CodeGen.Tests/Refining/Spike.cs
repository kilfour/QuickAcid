using QuickMGenerate;
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
    [Fact(Skip = "demo")]
    public void Lets_see_where_this_vein_leads()
    {
        var script =
            from account in "Account".Tracked(() => new Account(), a => a.Balance.ToString())
            from _ in "ops".Choose(
                from depositAmount in "deposit".Input(MGen.Int(0, 100))
                from act in "account.Deposit:deposit".Act(() => account.Deposit(depositAmount))
                select Acid.Test,
                from withdrawAmount in "withdraw".Input(MGen.Int(0, 100))
                from withdraw in "account.Withdraw:withdraw".Act(() => account.Withdraw(withdrawAmount))
                select Acid.Test
            )
            from _s1 in "No Overdraft: account.Balance >= 0".Spec(() => account.Balance >= 0)
            from _s2 in "Balance Has Maximum: account.Balance <= 100".Spec(() => account.Balance <= 100)
            select Acid.Test;

        //run.TheWohlwillProcess(20, 20);
    }
}
