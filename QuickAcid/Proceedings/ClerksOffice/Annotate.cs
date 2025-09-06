using QuickAcid.TheyCanFade;

namespace QuickAcid.Proceedings.ClerksOffice;

public static class Annotate
{
    public static CaseFile TheCaseFile(CaseFile caseFile, Memory Memory, List<int> ExecutionNumbers)
    {
        foreach (var executionNumber in ExecutionNumbers.ToList())
        {
            caseFile.AddExecutionDiagnosisDeposition(GetDiagnosisExecutionDeposition(Memory, executionNumber));
        }
        return caseFile;
    }

    private static DiagnosisExecutionDeposition GetDiagnosisExecutionDeposition(Memory memory, int executionNumber)
    {
        var executionDeposition = new DiagnosisExecutionDeposition(executionNumber);
        GetDiagnosisDepositions(executionDeposition, memory.DiagnosisFor(executionNumber));
        return executionDeposition;
    }

    private static void GetDiagnosisDepositions(
        DiagnosisExecutionDeposition executionDeposition,
        Dictionary<string, List<string>> diagnosis)
    {
        foreach (var (key, val) in diagnosis)
        {
            executionDeposition.AddDiagnosisDeposition(new DiagnosisDeposition(key, val));
        }
    }
}
