using QuickAcid.Proceedings.ClerksOffice;

namespace QuickAcid.Proceedings;

public class Verdict
{
    public int OriginalRunExecutionCount { get; private set; } = 0;
    public int ExecutionCount { get; private set; } = 0;
    public int ShrinkCount { get; private set; } = 0;
    public int Seed { get; private set; } = 0;

    public FailureDeposition FailureDeposition { get; }

    public List<ExecutionDeposition> PassedSpecDepositions { get; } = [];

    public List<ExecutionDeposition> ExecutionDepositions { get; } = [];

    public TestMethodInfoDeposition? TestMethodInfoDeposition { get; private set; }

    private Verdict(FailureDeposition failureDeposition) { FailureDeposition = failureDeposition; }

    public static Verdict FromDossier(Dossier dossier)
    {
        return new Verdict(GetFailureDeposition(dossier))
        {
            OriginalRunExecutionCount = dossier.OriginalRunExecutionCount,
            ExecutionCount = dossier.ExecutionNumbers.Count,
            ShrinkCount = dossier.ShrinkCount,
            Seed = dossier.Seed
        };
    }

    private static FailureDeposition GetFailureDeposition(Dossier dossier)
    {
        return (dossier.Exception == null)
            ? (dossier.FailingSpec == null)
                ? new AssayerDeposition(dossier.AssayerSpec!) :
                new FailedSpecDeposition(dossier.FailingSpec!)
            : new ExceptionDeposition(dossier.Exception);
    }

    public Verdict AddExecutionDeposition(ExecutionDeposition executionDepostion)
    {
        var last = ExecutionDepositions.LastOrDefault();
        if (last == null || !last.Collapsed(executionDepostion))
            ExecutionDepositions.Add(executionDepostion);
        return this;
    }

    public Verdict AddTestMethodDisposition(TestMethodInfoDeposition testMethodInfoDeposition)
    {
        TestMethodInfoDeposition = testMethodInfoDeposition;
        return this;
    }
}
