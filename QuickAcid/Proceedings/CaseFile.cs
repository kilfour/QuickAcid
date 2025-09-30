
using QuickAcid.Shrinking;

namespace QuickAcid.Proceedings;

public class CaseFile
{
    private CaseFile(Verdict verdict) { Verdict = verdict; }

    public static CaseFile Empty()
    {
        return new CaseFile(null!);
    }

    public Verdict Verdict { get; private set; }
    public bool HasVerdict() { return Verdict != null; }
    public static CaseFile WithVerdict(Verdict verdict)
    {
        return new CaseFile(verdict);
    }

    public List<RunDeposition> RunDepositions { get; } = [];
    public CaseFile AddRunDeposition(RunDeposition runDeposition)
    {
        RunDepositions.Add(runDeposition);
        return this;
    }

    public List<PassedSpecDeposition> PassedSpecDepositions { get; } = [];
    public CaseFile AddPassedSpecDeposition(PassedSpecDeposition passedSpecDeposition)
    {
        PassedSpecDepositions.Add(passedSpecDeposition);
        return this;
    }

    public List<DiagnosisExecutionDeposition> DiagnosisExecutionDepositions { get; } = [];
    public CaseFile AddExecutionDiagnosisDeposition(DiagnosisExecutionDeposition diagnosisExecutionDeposition)
    {
        DiagnosisExecutionDepositions.Add(diagnosisExecutionDeposition);
        return this;
    }

    public List<ShrinkTrace> ShrinkTraces { get; internal set; } = [];
}