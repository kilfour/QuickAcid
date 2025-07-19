using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickExplainIt.Text;

namespace QuickAcid.Tests.Proceedings.InputDepositions;

public class InputDepositionsTests : DepositionTest
{
    [Fact]
    public void None()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)));
        var reader = Transcribe(caseFile);
        EndOfContent(reader);
    }

    [Fact]
    public void One_Int()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddInputDeposition(new InputDeposition("PropertyName", 42))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Executed (1):", reader.NextLine());
        Assert.Equal("   - Input: PropertyName = 42", reader.NextLine());
        EndOfContent(reader);
    }

    [Fact]
    public void One_String()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddInputDeposition(new InputDeposition("PropertyName", "42"))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Executed (1):", reader.NextLine());
        Assert.Equal("   - Input: PropertyName = \"42\"", reader.NextLine());
        EndOfContent(reader);
    }

    [Fact]
    public void Two()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddInputDeposition(new InputDeposition("One", 42))
                    .AddInputDeposition(new InputDeposition("Two", "42"))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal(" Executed (1):", reader.NextLine());
        Assert.Equal("   - Input: One = 42", reader.NextLine());
        Assert.Equal("   - Input: Two = \"42\"", reader.NextLine());
        EndOfContent(reader);
    }
}