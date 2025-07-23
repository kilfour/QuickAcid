
namespace QuickAcid.Proceedings;

public class CaseFile
{
    public List<RunDeposition> RunDepositions { get; } = [];
    public Verdict Verdict { get; private set; }
    private CaseFile(Verdict verdict) { Verdict = verdict; }
    public CaseFile AddRunDeposition(RunDeposition runDeposition)
    {
        RunDepositions.Add(runDeposition);
        return this;
    }

    public bool HasVerdict() { return Verdict != null; }

    public static CaseFile Empty()
    {
        return new CaseFile(null!);
    }

    public static CaseFile WithVerdict(Verdict verdict)
    {
        return new CaseFile(verdict);
    }
};