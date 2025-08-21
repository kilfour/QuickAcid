using System.Diagnostics;
using QuickAcid.Proceedings;
using QuickAcid.Proceedings.ClerksOffice;
using QuickPulse.Explains.Text;

namespace QuickAcid.Tests.Proceedings;

public abstract class DepositionTest
{
    protected virtual bool IgnoreVerdictHeader { get; init; } = true;
    protected virtual bool IgnoreFailingSpec { get; init; } = true;
    protected Dossier Dossier { get; } =
        new Dossier(
                FailingSpec: "Some Invariant",
                Exception: null,
                AssayerSpec: null,
                OriginalRunExecutionCount: 10,
                ExecutionNumbers: [1, 2, 3, 4],
                ShrinkCount: 1,
                Seed: 12345678
            );

    protected LinesReader Transcribe(CaseFile caseFile)
    {
        var result = TheClerk.Transcribes(caseFile);
        var reader = LinesReader.FromText(result);
        if (IgnoreVerdictHeader)
            reader.Skip(4);
        return reader;

    }

    [StackTraceHidden]
    protected void EndOfContent(LinesReader reader)
    {
        if (IgnoreFailingSpec)
            reader.Skip(3);
        Assert.True(reader.EndOfContent());
    }
}