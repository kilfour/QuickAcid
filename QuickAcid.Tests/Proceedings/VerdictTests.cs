using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickExplainIt.Text;

namespace QuickAcid.Tests.Proceedings;

public class VerdictTests
{
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
        var result = Clerk.Transcribe(caseFile);

        var reader = LinesReader.FromText(result);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Original failing run:    10 executions.", reader.NextLine());
        Assert.Equal(" Minimal failing case:    4 executions after (1 shrink).", reader.NextLine());
        Assert.Equal(" Seed:                    12345678.", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfContent());
        Assert.True(reader.EndOfContent());
    }
}