using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings.ActionDepositions;

public class DiagnosisDepositionsTests : DepositionTest
{
    public DiagnosisDepositionsTests()
    {
        IgnoreVerdictHeader = false;
    }

    [Fact]
    public void OneExecution()
    {
        var caseFile = CaseFile.Empty()
            .AddExecutionDiagnosisDeposition(new DiagnosisExecutionDeposition(1)
                .AddDiagnosisDeposition(new DiagnosisDeposition("diag 1", ["msg 1-1", "msg 1-2"]))
                .AddDiagnosisDeposition(new DiagnosisDeposition("diag 2", ["msg 2-1", "msg 2-2"])));
        var reader = Transcribe(caseFile);
        //Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Diagnosis (1):", reader.NextLine());
        Assert.Equal("   - diag 1:", reader.NextLine());
        Assert.Equal("     -> msg 1-1", reader.NextLine());
        Assert.Equal("     -> msg 1-2", reader.NextLine());
        Assert.Equal("   - diag 2:", reader.NextLine());
        Assert.Equal("     -> msg 2-1", reader.NextLine());
        Assert.Equal("     -> msg 2-2", reader.NextLine());
        EndOfContent(reader);
    }

    [Fact]
    public void TwoExecutions()
    {
        var caseFile = CaseFile.Empty()
            .AddExecutionDiagnosisDeposition(new DiagnosisExecutionDeposition(1)
                .AddDiagnosisDeposition(new DiagnosisDeposition("diag 1", ["msg 1-1", "msg 1-2"])))
            .AddExecutionDiagnosisDeposition(new DiagnosisExecutionDeposition(2)
                .AddDiagnosisDeposition(new DiagnosisDeposition("diag 2", ["msg 2-1", "msg 2-2"])));
        var reader = Transcribe(caseFile);
        // Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Diagnosis (1):", reader.NextLine());
        Assert.Equal("   - diag 1:", reader.NextLine());
        Assert.Equal("     -> msg 1-1", reader.NextLine());
        Assert.Equal("     -> msg 1-2", reader.NextLine());
        Assert.Equal("  Diagnosis (2):", reader.NextLine());
        Assert.Equal("   - diag 2:", reader.NextLine());
        Assert.Equal("     -> msg 2-1", reader.NextLine());
        Assert.Equal("     -> msg 2-2", reader.NextLine());
        EndOfContent(reader);
    }
}