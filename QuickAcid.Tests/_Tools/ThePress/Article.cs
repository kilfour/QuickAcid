using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class Article
{
    private readonly CaseFile caseFile;

    public Article(CaseFile caseFile) => this.caseFile = caseFile;

    public bool VerdictReached() => caseFile.HasVerdict();

    public Exception Exception()
    {
        if (caseFile.Verdict.FailureDeposition is ExceptionDeposition exceptionDeposition)
        {
            return exceptionDeposition.Exception;
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

    public ExecutionArticle Execution(int number)
    {
        return new ExecutionArticle(caseFile.Verdict.ExecutionDepositions[number - 1]);
    }

    public WordCount Total()
    {
        return new WordCount(caseFile.Verdict.ExecutionDepositions);
    }
}
