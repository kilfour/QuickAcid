using QuickMGenerate;

namespace QuickAcid.Examples;

public class BankAccountExample
{
    [Fact]
    public void AccountPropertiesShouldHold()
    {
        TheTest.Verify(20.Runs(), 50.ActionsPerRun());
    }

    private static readonly QAcidRunner<Acid> TheTest =
        from account in "account".OnceOnlyInput(() => new BankAccount())
        from action in "account ops".Choose(
            Deposit(account),
            Withdraw(account))
        select Acid.Test;

    private static QAcidRunner<Acid> Deposit(BankAccount account)
    {
        return
            from amount in "deposit amount".ShrinkableInput(MGen.Int(1, 100))
            from act in "Deposit".Act(() => account.Deposit(amount))
            from spec in "Balance increased".Spec(() => account.Balance >= amount)
            select Acid.Test;
    }

    private static QAcidRunner<Acid> Withdraw(BankAccount account)
    {
        return (
            from amount in "withdraw amount".ShrinkableInput(MGen.Int(0, 150))
            from act in "Withdraw".Act(() => account.Withdraw(amount))
            from spec1 in "No overdraft".Spec(() => account.Balance >= 0)
            from spec2 in "Never negative".Spec(() => account.Balance >= 0)
            select Acid.Test)
            .If(() => account.Balance > 0); // only valid when there's something to withdraw
    }

    public class BankAccount
    {
        private int _balance;
        public int Balance => _balance;

        public void Deposit(int amount)
        {
            if (amount < 0) throw new ArgumentException("Cannot deposit negative");
            _balance += amount;
        }

        public void Withdraw(int amount)
        {
            if (amount > _balance)
            {
                // simulate a bug: withdrawal not prevented
                //  _balance -= amount;
            }
            else
            {
                _balance -= amount;
            }
        }
    }
}