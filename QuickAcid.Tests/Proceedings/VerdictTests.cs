using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickExplainIt.Text;

namespace QuickAcid.Tests.Proceedings;

public class VerdictTests : DepositionTest
{
    public VerdictTests() { IgnoreVerdictHeader = false; }
    [Fact]
    public void One()
    {
        var caseFile = new CaseFile().WithVerdict(Verdict.FromDossier(Dossier));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Original failing run:    10 executions.", reader.NextLine());
        Assert.Equal(" Minimal failing case:    4 executions after (1 shrink).", reader.NextLine());
        Assert.Equal(" Seed:                    12345678.", reader.NextLine());
        EndOfContent(reader);
    }
}