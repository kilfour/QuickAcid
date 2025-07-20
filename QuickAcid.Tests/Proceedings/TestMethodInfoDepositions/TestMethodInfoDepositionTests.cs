using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings.TestMethodInfoDepositions;

public class TestMethodInfoDepositionTests : DepositionTest
{
    public TestMethodInfoDepositionTests()
    {
        IgnoreVerdictHeader = false;
    }

    [Fact]
    public void One()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier)
                .AddTestMethodDisposition(new TestMethodInfoDeposition("method name", "source/file.cs", 42)));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Test:                    method name", reader.NextLine());
        Assert.Equal(" Location:                source/file.cs:42:1", reader.NextLine());
        Assert.Equal(" Original failing run:    10 executions", reader.NextLine());
        Assert.Equal(" Minimal failing case:    4 executions (after 1 shrink)", reader.NextLine());
        Assert.Equal(" Seed:                    12345678", reader.NextLine());
        Assert.Equal(" ═════════════════════════════════", reader.NextLine());
        Assert.Equal("  ❌ Spec Failed: Some Invariant", reader.NextLine());
        Assert.Equal(" ═════════════════════════════════", reader.NextLine());
        EndOfContent(reader);
    }
}