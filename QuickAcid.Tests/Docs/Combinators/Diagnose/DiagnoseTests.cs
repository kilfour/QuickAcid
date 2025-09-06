using QuickPulse.Explains;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using QuickAcid.Tests._Tools.ThePress;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Trace;

[DocFile]
[DocContent(
@"**Diagnose(...)** Used to add information from the script to the QuickAcid report.
")]
public class DiagnoseTests
{
    [Fact]
    public void Diagnose_usage()
    {
        var script =
            from _ in "act".Act(() => { })
            from __ in "Info Key".Diagnose(a => "Extra words")
            from ___ in "Info Key".DiagnoseIf(a => a.ExecutionId == 1, a => "More Extra words")
            select Acid.Test;

        var article = TheJournalist.Unearths(
            QState.Run("Diagnose", script)
            .WithOneRun()
            .And(2.ExecutionsPerRun()));

        Assert.Equal(2, article.DiagnosisExecutionsCount);

        Assert.Equal(1, article.DiagnoseExecutions(1).DiagnosisCount);
        Assert.Equal(1, article.DiagnoseExecutions(1).Diagnosis(1).TraceCount);
        Assert.Equal("Extra words", article.DiagnoseExecutions(1).Diagnosis(1).Read(1));

        Assert.Equal(1, article.DiagnoseExecutions(2).DiagnosisCount);
        Assert.Equal(2, article.DiagnoseExecutions(2).Diagnosis(1).TraceCount);
        Assert.Equal("Extra words", article.DiagnoseExecutions(2).Diagnosis(1).Read(1));
        Assert.Equal("More Extra words", article.DiagnoseExecutions(2).Diagnosis(1).Read(2));
    }
}

