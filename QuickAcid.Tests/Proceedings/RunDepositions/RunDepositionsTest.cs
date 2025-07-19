using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickExplainIt.Text;

namespace QuickAcid.Tests.Proceedings.ExecutionDepositions;

public class RunDepositionsTest : DepositionTest
{
    [Fact]
    public void None()
    {
        var caseFile = new CaseFile().WithVerdict(Verdict.FromDossier(Dossier));
        var reader = Transcribe(caseFile);
        EndOfContent(reader);
    }

    [Fact]
    public void One()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier))
            .AddRunDeposition(new RunDeposition()
                .AddExecutionDeposition(new ExecutionDeposition(1)));
        var reader = Transcribe(caseFile);
        EndOfContent(reader);
    }

    [Fact]
    public void Two()
    {
        var caseFile = new CaseFile()
            .WithVerdict(Verdict.FromDossier(Dossier))
            .AddRunDeposition(new RunDeposition()
                .AddExecutionDeposition(new ExecutionDeposition(1))
                .AddExecutionDeposition(new ExecutionDeposition(2)));
        var reader = Transcribe(caseFile);
        EndOfContent(reader);
    }
}