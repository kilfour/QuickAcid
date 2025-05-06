using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy;

public static class Chapter { public const string Order = "1-50-50"; }

[Doc(Order = Chapter.Order, Caption = "Feedback Shrinking", Content =
@"A.k.a.: What if it fails but the run does not contain the minimal fail case ? 
")]
public class FeedbackShrinkingTests
{
    [Fact]
    public void Spike()
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

        var report = new QState(run).Observe(50);
    }

    public class Account
    {
        public int Balance = 0;
        public void Deposit(int amount) { Balance += amount; }
        public void Withdraw(int amount) { Balance -= amount; }
    }
}

