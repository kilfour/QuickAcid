using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickExplainIt.Text;

namespace QuickAcid.Tests.Proceedings;

public abstract class DepositionTest
{
    protected bool IgnoreVerdictHeader { get; init; } = true;
    protected bool IgnoreFailingSpec { get; init; } = true;

    protected LinesReader Transcribe(CaseFile caseFile)
    {
        var result = Clerk.Transcribe(caseFile);
        var reader = LinesReader.FromText(result);
        if (IgnoreVerdictHeader)
            reader.Skip(4);
        return reader;

    }

    protected void EndOfContent(LinesReader reader)
    {
        if (IgnoreFailingSpec)
            reader.Skip(3);
        Assert.True(reader.EndOfContent());
    }
}