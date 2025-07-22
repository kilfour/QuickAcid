using QuickAcid.Proceedings;

namespace QuickAcid.Tests._Tools.ThePress;

public class Article
{
    private readonly CaseFile caseFile;

    public Article(CaseFile caseFile) => this.caseFile = caseFile;

    public ExecutionArticle Execution(int number)
    {
        return new ExecutionArticle(caseFile.Verdict.ExecutionDepositions[number - 1]);
    }

    public WordCount Total()
    {
        return new WordCount(caseFile.Verdict.ExecutionDepositions);
    }
}
