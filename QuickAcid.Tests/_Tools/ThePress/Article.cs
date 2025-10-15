using System.Diagnostics;
using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class Article(CaseFile caseFile)
{
    private readonly CaseFile caseFile = caseFile;

    public CaseFile CaseFile { get; } = caseFile;
    public bool VerdictReached() => caseFile.HasVerdict();

    public string AssayerDisagrees()
    {
        if (caseFile.Verdict.FailureDeposition is AssayerDeposition assayerDeposition)
        {
            return assayerDeposition.FailedSpec;
        }
        return null!;
    }

    public string FailedSpec()
    {
        if (caseFile.Verdict.FailureDeposition is FailedSpecDeposition failedSpecDeposition)
        {
            return failedSpecDeposition.FailedSpec;
        }
        return null!;
    }

    public Exception Exception()
    {
        if (caseFile.Verdict.FailureDeposition is ExceptionDeposition exceptionDeposition)
        {
            return exceptionDeposition.Exception;
        }
        return null!;
    }

    [StackTraceHidden]
    public ExecutionArticle Execution(int number)
        => new(caseFile.Verdict.ExecutionDepositions[number - 1]);

    [StackTraceHidden]
    public DiagnosisExecutionArticle DiagnoseExecutions(int number) =>
        new(caseFile.DiagnosisExecutionDepositions[number - 1]);
    public int DiagnosisExecutionsCount => caseFile.DiagnosisExecutionDepositions.Count;


    [StackTraceHidden]
    public PassedSpecArticle PassedSpec(int number)
        => new(caseFile.PassedSpecDepositions[number - 1]);


    public WordCount Total()
        => new(caseFile.Verdict.ExecutionDepositions);

    public int Seed() => caseFile.Verdict.Seed;


}
