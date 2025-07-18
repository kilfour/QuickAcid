using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickExplainIt.Text;

namespace QuickAcid.Tests.Proceedings.InputDepositions;

public class InputDepositionsTests
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
    public void One_Int()
    {
        var caseFile = new CaseFile()
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddInputDeposition(new InputDeposition("PropertyName", 42)));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Executed (1):", reader.NextLine());
        Assert.Equal("   - Input: PropertyName = 42", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void One_String()
    {
        var caseFile = new CaseFile()
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddInputDeposition(new InputDeposition("PropertyName", "42")));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Executed (1):", reader.NextLine());
        Assert.Equal("   - Input: PropertyName = \"42\"", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }

    [Fact]
    public void Two()
    {
        var caseFile = new CaseFile()
            .AddExecutionDeposition(new ExecutionDeposition(1)
                .AddInputDeposition(new InputDeposition("One", 42))
                .AddInputDeposition(new InputDeposition("Two", "42")));
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Executed (1):", reader.NextLine());
        Assert.Equal("   - Input: One = 42", reader.NextLine());
        Assert.Equal("   - Input: Two = \"42\"", reader.NextLine());
        Assert.True(reader.EndOfContent());
    }
}