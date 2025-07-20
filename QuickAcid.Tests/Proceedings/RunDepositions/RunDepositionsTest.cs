using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings.ExecutionDepositions;

public class RunDepositionsTest : DepositionTest
{
    public RunDepositionsTest() { IgnoreVerdictHeader = false; }// not ignoring verdict, but messes with rundepositions 

    [Fact]
    public void One()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier))
            .AddRunDeposition(new RunDeposition("A LABEL")
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddActionDeposition(new ActionDeposition("Something I Did"))
                    .AddInputDeposition(new InputDeposition("PropertyName", 42))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" A LABEL", reader.NextLine());
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Executed (1): Something I Did", reader.NextLine());
        Assert.Equal("   - Input: PropertyName = 42", reader.NextLine());
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Original failing run:    10 executions", reader.NextLine());
        Assert.Equal(" Minimal failing case:    4 executions (after 1 shrink)", reader.NextLine());
        Assert.Equal(" Seed:                    12345678", reader.NextLine());
        Assert.Equal(" ═════════════════════════════════", reader.NextLine());
        Assert.Equal("  ❌ Spec Failed: Some Invariant", reader.NextLine());
        Assert.Equal(" ═════════════════════════════════", reader.NextLine());
        Assert.True(reader.EndOfContent());
        reader.Skip(4); // ignore verdict
        EndOfContent(reader);
    }
}