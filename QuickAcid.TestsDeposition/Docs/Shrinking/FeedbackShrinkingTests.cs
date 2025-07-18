using QuickAcid.Reporting;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;

namespace QuickAcid.TestsDeposition.Docs.Shrinking;

public static class Chapter { public const string Order = "1-50-50"; }

// [Doc(Order = Chapter.Order, Caption = "Feedback Shrinking", Content =
// @"A.k.a.: What if it fails but the run does not contain the minimal fail case ? 
// ")]
public class FeedbackShrinkingTests
{
    [Fact(Skip = "disabled feedback shrinking")]
    public void Normal_Shrinking()
    {
        QDiagnosticState.RunInformation? info = null;
        Report? report = null;
        var numberOfActionEntries = 0;
        while (numberOfActionEntries < 5)
        {
            var firstRun = GetRun(Fuzz.Constant(20));
            var state = new QDiagnosticState(firstRun).AllowShrinking(false).Observe(50);
            info = state.GetRunInformation();
            report = state.GetReport();
            Assert.NotNull(report);
            numberOfActionEntries = report.OfType<ReportExecutionEntry>().Count();
        }
        var shrinkingRun = GetRun(Fuzz.Int());
        report = new QDiagnosticState(shrinkingRun).Shrink(info!).GetReport();
        Assert.NotNull(report);
        Assert.Equal(2, report.OfType<ReportExecutionEntry>().Count());
    }

    [Fact(Skip = "disabled feedback shrinking")]
    public void Feedback_Shrinking()
    {
        QDiagnosticState.RunInformation? info = null;
        Report? report = null;
        var numberOfActionEntries = 0;
        while (numberOfActionEntries < 5)
        {
            var firstRun = GetRun(Fuzz.Constant(20));
            var state = new QDiagnosticState(firstRun).AllowShrinking(false).Observe(50);
            info = state.GetRunInformation();
            report = state.GetReport();
            Assert.NotNull(report);
            numberOfActionEntries = report.OfType<ReportExecutionEntry>().Count();
        }

        var shrinkingRun = GetRun(Fuzz.Int());
        report = new QDiagnosticState(shrinkingRun).WithFeedback().Shrink(info!).GetReport();
        Assert.NotNull(report);
        // Assert.Single(report.OfType<ReportExecutionEntry>());
        // Assert.Single(report.OfType<ReportInputEntry>());
        // var entry = report.Single<ReportInputEntry>();
        // Assert.Equal("withdraw", entry.Key);
        // TODO reactivate using shrinktrace Intent == Replaced
        //Assert.True(Convert.ToInt32(entry.Value!) > 30);
    }

    private QAcidScript<Acid> GetRun(Generator<int> intGenerator)
    {
        return
            from account in "Account".Tracked(
                () => new Account(),
                a => a.Balance.ToString())
            from box in "flag".StashedValue(false)
            from _ in "ops".Choose(
                from depositAmount in "deposit".Input(intGenerator)
                from act in "account.Deposit".ActIf(
                    () => !box.Value, () => { box.Value = true; account.Deposit(depositAmount); })
                select Acid.Test,
                from withdrawAmount in "withdraw".Input(intGenerator)
                from withdraw in "account.Withdraw:withdraw".Act(
                    () =>
                    {
                        account.Withdraw(withdrawAmount);
                    })
                select Acid.Test
            )
            from spec in "No_Overdraft: account.Balance >= 0".Spec(() => account.Balance >= 0)
            select Acid.Test;
    }

    public class Account
    {
        public int Balance = 30;
        public void Deposit(int amount) { Balance += amount; }
        public void Withdraw(int amount) { Balance -= amount; }
    }
}

