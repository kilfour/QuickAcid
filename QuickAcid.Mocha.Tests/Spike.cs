using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate;
using Jint;
using QuickPulse.Instruments;

namespace QuickAcid.Mocha.Tests;

public class Spike
{
    [Fact(Skip = "demo")]
    public void AcidTest()
    {
        var path = SolutionLocator.FindSolutionRoot() + "\\QuickAcid.Mocha.Tests";
        var module = From.Path(path).AndFile("./account.js");
        var script =
            from account in "Account".Tracked(
                () => module.Construct("Account"),
                a => a.Call("getBalance").AsNumber().ToString())
            from _ in "ops".Choose(
                from depositAmount in "deposit".Input(MGen.Int())
                from act in "account.Deposit".Act(
                    () => account.Call("deposit", depositAmount))
                select Acid.Test,
                from withdrawAmount in "withdraw".Input(MGen.Int())
                from withdraw in "account.Withdraw:withdraw".Act(
                    () => account.Call("withdraw", withdrawAmount))
                select Acid.Test
            )
            from spec in "No_Overdraft: account.Balance >= 0".Spec(
                () => account.Call("getBalance").AsNumber() >= 0)
            select Acid.Test;
        10.Times(() => new QState(script).Testify(20));
    }

    [Fact]
    public void Account_DepositAndWithdraw_ProducesCorrectBalance()
    {
        var path = SolutionLocator.FindSolutionRoot() + "\\QuickAcid.Mocha.Tests";
        var engine = new Engine(opts => { opts.EnableModules(path); });
        var accountModule = engine.Modules.Import("./account.js");
        var module = From.Path(path).AndFile("./account.js");
        var account = module.Construct("Account");

        var balance = account.Call("getBalance");
        Assert.Equal(0, balance);

        account.Call("deposit", 100);
        balance = account.Call("getBalance");
        Assert.Equal(100, balance);

        account.Call("withdraw", 30);
        balance = account.Call("getBalance");
        Assert.Equal(70, balance);
    }
}
