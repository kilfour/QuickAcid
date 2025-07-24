
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

    public List<ExtraDeposition> ExtraDepositions { get; } = [];
    public CaseFile AddExtraDeposition(ExtraDeposition extraDeposition)
    {
        ExtraDepositions.Add(extraDeposition);
        return this;
    }
};