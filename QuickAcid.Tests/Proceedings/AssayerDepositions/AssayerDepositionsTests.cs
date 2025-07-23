using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;

namespace QuickAcid.Tests.Proceedings;

public class AssayerDepositionsTests : DepositionTest
{
    public AssayerDepositionsTests() { IgnoreVerdictHeader = false; IgnoreFailingSpec = false; }

    [Fact]
    public void One()
    {
        var caseFile = CaseFile.WithVerdict(Verdict.FromDossier(new Dossier(
                FailingSpec: null,
                Exception: null,
                AssayerSpec: "Some Invariant",
                OriginalRunExecutionCount: 10,
                ExecutionNumbers: [1, 2, 3, 4],
                ShrinkCount: 0,
                Seed: 12345678
            )));

        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Original failing run:    10 executions", reader.NextLine());
        Assert.Equal(" Seed:                    12345678", reader.NextLine());
        Assert.Equal(" ════════════════════════════════════════════", reader.NextLine());
        Assert.Equal("  ❌  The Assayer Disagrees: Some Invariant", reader.NextLine());
        Assert.Equal(" ════════════════════════════════════════════", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}