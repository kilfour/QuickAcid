using QuickAcid.Proceedings;

namespace QuickAcid.Tests.Proceedings.ExecutionDepositions;

public class ExecutionDepositionsTests : DepositionTest
{
    [Fact]
    public void None_Empty()
    {
        var caseFile = new CaseFile().WithVerdict(new Verdict(new FailedSpecDeposition("Some Invariant")));
        var reader = Transcribe(caseFile);
        EndOfContent(reader);
    }

    [Fact]
    public void One_Empty()
    {
        var caseFile = new CaseFile()
            .WithVerdict(new Verdict(new FailedSpecDeposition("Some Invariant"))
                .AddExecutionDeposition(new ExecutionDeposition(1)));
        var reader = Transcribe(caseFile);
        EndOfContent(reader);
    }

    [Fact]
    public void Two_Empty()
    {
        var caseFile = new CaseFile()
            .WithVerdict(new Verdict(new FailedSpecDeposition("Some Invariant"))
                .AddExecutionDeposition(new ExecutionDeposition(1))
                .AddExecutionDeposition(new ExecutionDeposition(2)));
        var reader = Transcribe(caseFile);
        EndOfContent(reader);
    }
}