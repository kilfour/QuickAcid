using QuickAcid.CodeGen;

namespace QuickAcid.Tests.CodeGen;

public class LinesReader
{
    private string[] lines;
    private int currentIndex = -1;

    private LinesReader(string text)
    {
        lines = text.Split(Environment.NewLine);
        if (lines.Count() > 0)
            currentIndex = 0;
    }

    public static LinesReader FromText(string text)
    {
        return new LinesReader(text);
    }

    public static LinesReader FromRun(QAcidRunner<Acid> runner)
    {
        var state = new QState(runner);
        var report = state.GenerateCode().AlwaysReport().Observe(1);
        return new LinesReader(report.Code);
    }

    public string NextLine()
    {
        if (currentIndex == -1) return "-- NO TEXT RECEIVED --";
        if (currentIndex > lines.Count() - 1) return "-- READ BEYOND THE TEXT --";
        var result = lines[currentIndex];
        currentIndex++;
        return result;
    }

    public LinesReader Skip()
    {
        currentIndex++;
        return this;
    }

    public void Skip(int linesToSkip)
    {
        currentIndex += linesToSkip;
    }

    public bool EndOfCode()
    {
        return currentIndex >= lines.Count();
    }
}