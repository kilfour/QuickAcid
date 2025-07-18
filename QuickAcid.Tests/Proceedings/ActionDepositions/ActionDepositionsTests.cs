using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickExplainIt.Text;

namespace QuickAcid.Tests.Proceedings.ActionDepositions;

public class ActionDepositionsTests
{
    [Fact]
    public void None()
    {
        var caseFile = new CaseFile()
            .AddExecutionDeposition(new ExecutionDeposition(1));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal("", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void One()
    {
        var caseFile = new CaseFile()
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddActionDeposition(new ActionDeposition("Something I Did")));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Executed (1): Something I Did", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Two()
    {
        var caseFile = new CaseFile()
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddActionDeposition(new ActionDeposition("Something I Did"))
                .AddActionDeposition(new ActionDeposition("Something Else")));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Executed (1): Something I Did, Something Else", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}