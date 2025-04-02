using QuickAcid.Tests.ZheZhools;

namespace QuickAcid.Tests;

public class SequenceTests
{
    [Fact]
    public void TwoActionsExceptionThrownOnFirst()
    {
        var run =
            AcidTestRun.FailedRun(2,
                "foobar".Sequence(
                    "foo".Act(() => throw new Exception()),
                    "bar".Act(() => { })));

        run.NumberOfReportEntriesIs(1);

        var entry = run.GetReportEntryAtIndex<QAcidReportActEntry>(0);
        Assert.Equal("foo", entry.Key);
        Assert.NotNull(entry.Exception);
    }

    [Fact]
    public void TwoActionsExceptionThrownOnSecond()
    {
        var run =
            AcidTestRun.FailedRun(2,
                "foobar".Sequence(
                    "foo".Act(() => { }),
                    "bar".Act(() => throw new Exception())));

        run.NumberOfReportEntriesIs(1);

        var entry = run.GetReportEntryAtIndex<QAcidReportActEntry>(0);
        Assert.Equal("bar", entry.Key);
        Assert.NotNull(entry.Exception);
    }

    [Fact]
    public void SequenceRepeatsIfMoreRunsSpecifiedTanActionsInSequence()
    {
        var count = 0;
        var run =
            AcidTestRun.FailedRun(100,
                "foobar".Sequence(
                    "foo".Act(() => { }),
                    "bar".Act(() => { if (count++ > 10) throw new Exception(); })));

        // -------------------------------------------------------------------------
        run.NumberOfReportEntriesIs(1);
        //  .__   __.   ______   .___________. _______ 
        //  |  \ |  |  /  __  \  |           ||   ____|
        //  |   \|  | |  |  |  | `---|  |----`|  |__   
        //  |  . `  | |  |  |  |     |  |     |   __|  
        //  |  |\   | |  `--'  |     |  |     |  |____ 
        //  |__| \__|  \______/      |__|     |_______|
        //           
        // This is correct behaviour but for better testing maybe it is better to get a hold of the unshrunken test run
        // -------------------------------------------------------------------------
    }
}