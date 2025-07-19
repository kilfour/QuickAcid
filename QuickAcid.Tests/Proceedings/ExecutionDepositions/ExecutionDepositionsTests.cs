using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickExplainIt.Text;

namespace QuickAcid.Tests.Proceedings.ExecutionDepositions;

public class ExecutionDepositionsTests
{
    [Fact]
    public void None()
    {
        var caseFile = new CaseFile().WithVerdict(new Verdict(new FailedSpecDeposition("Some Invariant")));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        reader.Skip(4); // <= ignore Verdict
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void One()
    {
        var caseFile = new CaseFile()
            .WithVerdict(new Verdict(new FailedSpecDeposition("Some Invariant"))
                .AddExecutionDeposition(new ExecutionDeposition(1)));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        reader.Skip(4); // <= ignore Verdict
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Two()
    {
        var caseFile = new CaseFile()
            .WithVerdict(new Verdict(new FailedSpecDeposition("Some Invariant"))
                .AddExecutionDeposition(new ExecutionDeposition(1))
                .AddExecutionDeposition(new ExecutionDeposition(2)));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        reader.Skip(4); // <= ignore Verdict
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}