using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings;

public class ExceptionDepositionsTests : DepositionTest
{
    [Fact]
    public void One()
    {
        var caseFile = new CaseFile()
            .WithVerdict(new Verdict(new ExceptionDeposition(new Exception("BOOM")))
            {
                OriginalRunExecutionCount = 10,
                ExecutionCount = 4,
                ShrinkCount = 1,
                Seed = 12345678
            });

        var reader = Transcribe(caseFile);
        reader.AsAssertsToLogFile();
        Assert.Equal(" ═══════════════════════════════════════════════════════════════════════════", reader.NextLine());
        Assert.Equal("  ❌ Exception Thrown: System.Exception: BOOM", reader.NextLine());
        Assert.Equal(" ═══════════════════════════════════════════════════════════════════════════", reader.NextLine());
        EndOfContent(reader);
    }
}