using QuickAcid.Proceedings;

namespace QuickAcid.TestsDeposition._Tools;

public class CaseFileInvestigation
{
    private readonly CaseFile caseFile;

    public CaseFileInvestigation(CaseFile caseFile) => this.caseFile = caseFile;

    public string TheFirstActionLabel() =>
        caseFile.Verdict.ExecutionDepositions
            .FirstOrDefault()?
            .ActionDepositions
            .FirstOrDefault()?
            .ActionLabel ?? "<no action>";
    public string TheSecondActionLabel() =>
        caseFile.Verdict.ExecutionDepositions
            .FirstOrDefault()?
            .ActionDepositions
            .Skip(1)
            .FirstOrDefault()?
            .ActionLabel ?? "<no action>";

    public string? FirstInputLabel() =>
        caseFile.Verdict.ExecutionDepositions
            .FirstOrDefault()?
            .InputDepositions
            .FirstOrDefault()?
            .Label;
}