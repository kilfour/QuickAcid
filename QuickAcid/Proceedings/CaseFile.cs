
namespace QuickAcid.Proceedings;

public class CaseFile
{
    public List<RunDeposition> RunDepositions { get; } = [];
    public Verdict? Verdict { get; private set; }

    public CaseFile AddRunDeposition(RunDeposition runDeposition)
    {
        RunDepositions.Add(runDeposition);
        return this;
    }

    public CaseFile WithVerdict(Verdict verdict)
    {
        Verdict = verdict;
        return this;
    }
};