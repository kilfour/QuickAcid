// using QuickAcid.TestsDeposition._Tools.Models;
// using QuickPulse.Explains;
// using QuickFuzzr;
// using QuickAcid.Tests._Tools.ThePress;

// namespace QuickAcid.TestsDeposition.Docs;

// [Doc(Order = Order, Caption = "QuickAcid", Content =
// @"Before we get in the technical details and/or sales pitch, 
// I feel it might help clear up what this thing actually does, and why it's useful, if I start with an example.  

// So here goes :  

// Given this naive, flawed, model:
// ```csharp
// public class Account
// {
//     public int Balance = 0;
//     public void Deposit(int amount) { Balance += amount; }
//     public void Withdraw(int amount) { Balance -= amount; }
// }
// ```
// And assuming that there exists a specification that an account holder should not be able to withdraw 
// funds that will cause the account balance to go below zero, we can write the following QuickAcid test.
// ```csharp
// var script =
//     from account in ""Account"".Tracked(() => new Account(), a => a.Balance.ToString())
//     from _ in ""ops"".Choose(
//         from depositAmount in ""deposit"".Input(Fuzz.Int(0, 10))
//         from act in ""account.Deposit"".Act(() => account.Deposit(depositAmount))
//         select Acid.Test,
//         from withdrawAmount in ""withdraw"".Input(Fuzz.Int(42, 42))
//         from withdraw in ""account.Withdraw:withdraw"".Act(() => account.Withdraw(withdrawAmount))
//         select Acid.Test
//     )
//     from spec in ""No_Overdraft: account.Balance >= 0"".Spec(() => account.Balance >= 0)
//     select Acid.Test;
// ```
// Running this test will produce the following output:
// ```
// QuickAcid.Bolts.FalsifiableException : QuickAcid Report:
//  ----------------------------------------
//  -- Property 'No_Overdraft' was falsified
//  -- Original failing run: 3 execution(s)
//  -- Shrunk to minimal case:  1 execution(s) (3 shrinks)
//  -- Seed: 1254808606
//  ----------------------------------------
//  RUN START :
//    => Account (tracked) : 0
//  ---------------------------
//  EXECUTE : account.Withdraw
//    - Input : withdraw = 42
//  ***************************
//   Spec Failed : No_Overdraft
//  ***************************
//  ```
//  Now you might say: ""But I can easily write a unit test for that, ... like so:""
//  ```csharp
// [Fact]
// public void Unit_test()
// {
//     var account = new Account();
//     account.Withdraw(42);
//     Assert.True(account.Balance >= 0);
// }
// ```
// Much simpler. Which is very true, and inadvertently, illustrates the point that property based testing is not meant to replace unit testing, but rather complement it.  
// You see, if you look closer at the report you will notice that the QuickAcid test tried three operations and furthermore 
// the number 42 you see in the output is not visible in the test. 
// Yet it still managed to pinpoint the minimal failing case in order to fail the constraint we put on our model.  
// So yes, if faced with this exact problem, write a unit test, but most problems aren't this obvious.  
// And in order to track those down you might think about calling in QuickAcid.

// *Would you like to know more ?*
// ")]
// public class QuickAcidTopLevel
// {
//     public const string Order = "1";

//     [Fact]
//     public void Example()
//     {
//         var script =
//             from account in "Account".Tracked(() => new Account())
//             from _ in "ops".Choose(
//                 from depositAmount in "deposit amount".Input(Fuzz.Int())
//                 from act in "Deposit".Act(() => account.Deposit(depositAmount))
//                 select Acid.Test,
//                 from withdrawAmount in "withdraw amount".Input(Fuzz.Int())
//                 from withdraw in "Withdraw".Act(() => account.Withdraw(withdrawAmount))
//                 select Acid.Test
//             )
//             from spec in "No Overdraft".Spec(() => account.Balance >= 0)
//             select Acid.Test;

//         TheJournalist.Exposes(() => QState.Run(script, 1047294985)
//             .Options(a => a with { FileAs = "QuickAcidTopLevel.Example" })
//            .WithOneRun()
//            .And(100.ExecutionsPerRun()));
//     }
// }