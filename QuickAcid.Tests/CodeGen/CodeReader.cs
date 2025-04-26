using QuickAcid.Bolts;
using QuickAcid.CodeGen;

namespace QuickAcid.Tests.CodeGen;

public class CodeReader
{
    private string[] lines;
    private int currentIndex = -1;

    public CodeReader(string code)
    {
        lines = code.Split(Environment.NewLine);
        if (lines.Count() > 0)
            currentIndex = 0;
    }

    public static CodeReader FromRun(QAcidRunner<Acid> runner)
    {
        var state = new QAcidState(runner);
        state.Run(1);
        return new CodeReader(Prospector.Pan(state));
    }

    public static CodeReader FromFailingRun(QAcidRunner<Acid> runner)
    {
        return new CodeReader(runner.ToCodeIfFailed(1, 1));
    }

    public static CodeReader FromFailingRunTryFiftyTimes(QAcidRunner<Acid> runner)
    {
        return new CodeReader(runner.ToCodeIfFailed(1, 50));
    }

    public string NextLine()
    {
        if (currentIndex == -1) return "-- NO CODE RECEIVED --";
        if (currentIndex > lines.Count() - 1) return "-- READ BEYOND THE CODE --";
        var result = lines[currentIndex];
        currentIndex++;
        return result;
    }
    public CodeReader Skip()
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