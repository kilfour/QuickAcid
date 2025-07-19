using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings;

public class FailedSpecDepositionsTests : DepositionTest
{
    public FailedSpecDepositionsTests() { IgnoreFailingSpec = false; }

    [Fact]
    public void One()
    {
        var caseFile = new CaseFile()
            .WithVerdict(new Verdict(new FailedSpecDeposition("Some Invariant"))
            {
                OriginalRunExecutionCount = 10,
                ExecutionCount = 4,
                ShrinkCount = 1,
                Seed = 12345678
            });

        var reader = Transcribe(caseFile);
        Assert.Equal(" ═════════════════════════════════", reader.NextLine());
        Assert.Equal("  ❌ Spec Failed: Some Invariant", reader.NextLine());
        Assert.Equal(" ═════════════════════════════════", reader.NextLine());
        EndOfContent(reader);
    }
}