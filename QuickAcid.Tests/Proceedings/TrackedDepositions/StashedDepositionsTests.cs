using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings.TrackedDepositions;

public class StashedDepositionsTests : DepositionTest
{

    [Fact]
    public void One()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddTrackedDeposition(new StashedDeposition("Tracked Value", "{ Property = 42}"))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("   => Tracked Value (tracked) : { Property = 42}", reader.NextLine());
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Executed (1):", reader.NextLine());
        EndOfContent(reader);
    }

    [Fact]
    public void Two()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddTrackedDeposition(new StashedDeposition("Tracked Value", "{ Property = 42}"))
                    .AddTrackedDeposition(new StashedDeposition("Other", "{ Truthy = false }"))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("   => Tracked Value (tracked) : { Property = 42}", reader.NextLine());
        Assert.Equal("   => Other (tracked) : { Truthy = false }", reader.NextLine());
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Executed (1):", reader.NextLine());
        EndOfContent(reader);
    }
}