using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings.ActionDepositions;

public class TraceDepositionsTests : DepositionTest
{
    [Fact]
    public void One()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddTraceDeposition(new TraceDeposition("Something", "I Traced"))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Executed (1):", reader.NextLine());
        Assert.Equal("   - Something: I Traced", reader.NextLine());
        EndOfContent(reader);
    }

    [Fact]
    public void Two()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddActionDeposition(new ActionDeposition("Something I Did"))
                    .AddActionDeposition(new ActionDeposition("Something Else"))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Executed (1): Something I Did, Something Else", reader.NextLine());
        EndOfContent(reader);
    }
}