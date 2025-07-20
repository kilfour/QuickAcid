using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings.ActionDepositions;

public class ActionDepositionsTests : DepositionTest
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
    public void One()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddActionDeposition(new ActionDeposition("Something I Did"))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Executed (1): Something I Did", reader.NextLine());
        EndOfContent(reader);
    }

    [Fact]
    public void Two()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier)
                .AddExecutionDeposition(new ExecutionDeposition(1)
                    .AddActionDeposition(new ActionDeposition("Something I Did"))
                    .AddActionDeposition(new ActionDeposition("Something Else"))));
        var reader = Transcribe(caseFile);
        Assert.Equal(" ──────────────────────────────────────────────────", reader.NextLine());
        Assert.Equal("  Executed (1): Something I Did, Something Else", reader.NextLine());
        EndOfContent(reader);
    }
}