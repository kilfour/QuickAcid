using QuickMGenerate;
using QuickAcid.Strike;

namespace QuickAcid.Examples.QuickStrike;

public class ItsOnlyAModel
{
    public class Account
    {
        public int Balance = 0;
        public void Deposit(int amount) { Balance += amount; }
        public void Withdraw(int amount) { Balance -= amount; }
    }

    [Fact(Skip = "WIP")]
    public void QuickAcid_as_a_unit_test_tool()
    {
        Test.This(
            () => new Account()
            , a => a.Balance.ToString())
            .Arrange(("withdraw", 42))
            .Act(Perform.This("withdraw", (Account account, int withdraw) => account.Withdraw(withdraw)))
            .Assert("No Overdraft", account => account.Balance >= 0)
            .UnitRun();
    }

    [Fact(Skip = "WIP")]
    public void QuickAcid_babies_first_pbt()
    {
        Test.This(() => new Account(), a => a.Balance.ToString())
            .Arrange(
                ("deposit", MGen.Int(0, 100)),
                ("withdraw", MGen.Int(0, 100)))
            .Act(
                Perform.This("deposit", (Account account, int deposit) => account.Deposit(deposit)),
                Perform.This("withdraw", (Account account, int withdraw) => account.Withdraw(withdraw)))
            .Assert("No Overdraft", account => account.Balance >= 0)
            .Assert("Balance Capped", account => account.Balance <= 100)
            .Run(1, 10);
    }
}