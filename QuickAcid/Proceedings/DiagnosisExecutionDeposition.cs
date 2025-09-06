

namespace QuickAcid.Proceedings;

public record DiagnosisExecutionDeposition
{
    public int ExecutionId { get; }

    public int Times { get; private set; } = 1;


    public List<DiagnosisDeposition> DiagnosisDepositions { get; } = [];

    public DiagnosisExecutionDeposition(int executionId)
    {
        ExecutionId = executionId;
    }

    public bool Collapsed(DiagnosisExecutionDeposition other)
    {
        if (!DiagnosisDepositions.SequenceEqual(other.DiagnosisDepositions))
            return false;
        Times++;
        return true;
    }

    public bool HasContent()
    {
        if (DiagnosisDepositions.Count > 0) return true;
        return false;
    }

    public DiagnosisExecutionDeposition AddDiagnosisDeposition(DiagnosisDeposition diagnosisDeposition)
    {
        DiagnosisDepositions.Add(diagnosisDeposition);
        return this;
    }
};