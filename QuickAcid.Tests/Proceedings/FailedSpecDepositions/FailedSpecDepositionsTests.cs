using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings;

public class FailedSpecDepositionsTests : DepositionTest
{
    public FailedSpecDepositionsTests() { IgnoreFailingSpec = false; }

    [Fact]
    public void One()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(Dossier));

        var reader = Transcribe(caseFile);
        Assert.Equal(" ═════════════════════════════════", reader.NextLine());
        Assert.Equal("  ❌ Spec Failed: Some Invariant", reader.NextLine());
        Assert.Equal(" ═════════════════════════════════", reader.NextLine());
        EndOfContent(reader);
    }
}