using QuickAcid.TestsDeposition._Tools;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen;

public static class Reader
{
    public static LinesReader FromRun(QAcidScript<Acid> run)
    {
        var code = new QCodeState(run).GenerateCode();
        var reader = LinesReader.FromText(code).TrimLines();
        reader.Skip(7);
        return reader;
    }
}