using QuickAcid.TestsDeposition._Tools;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen;

public static class Reader
{
    public static LinesReader FromRun(QAcidRunner<Acid> run)
    {
        var code = new QState(run).GenerateCode().AlwaysReport().ObserveOnce().Code;
        var reader = LinesReader.FromText(code).TrimLines();
        reader.Skip(7);
        return reader;
    }
}