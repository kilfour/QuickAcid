using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings;

public class ExceptionDepositionsTests : DepositionTest
{
    [Fact]
    public void One()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(
                Dossier with { FailingSpec = null, Exception = new Exception("BOOM") }));

        var reader = Transcribe(caseFile);
        Assert.Equal(" ═══════════════════════════════════════════════════════════════════════════", reader.NextLine());
        Assert.Equal("  ❌ Exception Thrown: System.Exception: BOOM", reader.NextLine());
        Assert.Equal(" ═══════════════════════════════════════════════════════════════════════════", reader.NextLine());
        EndOfContent(reader);
    }
}