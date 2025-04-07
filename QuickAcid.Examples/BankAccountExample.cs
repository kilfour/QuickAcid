// using QuickMGenerate;
// namespace QuickAcid.Examples;

// public class BankAccountExample : IClassFixture<QAcidLoggingFixture>
// {
//     [Fact(Skip = "Explicit")]
//     public void AccountPropertiesShouldHold()
//     {
//         TheTest.Verify(20.Runs(), 50.ActionsPerRun());
//     }

//     [Fact(Skip = "Explicit")]
//     public void BankAccount_ShouldPreventOverdraft()
//     {
//         var account = new BankAccount();
//         account.Deposit(19);
//         account.Withdraw(105);
//         Assert.True(account.Balance >= 0);
//     }

//     private static readonly QAcidRunner<Acid> TheTest =
//         from account in "account".AlwaysReportedInput(() => new BankAccount())
//         from action in "account ops".Choose(
//             Deposit(account),
//             Withdraw(account))
//         from spec in "Never negative".Spec(() => account.Balance >= 0)
//         select Acid.Test;

//     private static QAcidRunner<Acid> Deposit(BankAccount account)
//     {
//         return
//             from amount in "deposit amount".ShrinkableInput(MGen.Int(1, 100))
//             from act in "Deposit".Act(() => account.Deposit(amount))
//             from spec in "Balance increased".Spec(() => account.Balance >= amount)
//             select Acid.Test;
//     }

//     private static QAcidRunner<Acid> Withdraw(BankAccount account)
//     {
//         return (
//             from amount in "withdraw amount".ShrinkableInput(MGen.Int(0, 150))
//             from act in "Withdraw".Act(() => account.Withdraw(amount))
//             from spec1 in "No overdraft".Spec(() => account.Balance >= 0)
//             select Acid.Test)
//             .If(() => account.Balance > 0); // only valid when there's something to withdraw
//     }


// }