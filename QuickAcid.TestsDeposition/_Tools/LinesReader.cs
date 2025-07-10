
namespace QuickAcid.TestsDeposition._Tools;

public class LinesReader
{
    private string[] lines;
    private int currentIndex = -1;
    private bool trimLines = false;

    private LinesReader(string text) : this(text.Split(Environment.NewLine)) { }

    private LinesReader(string[] lines)
    {
        this.lines = lines;
        if (lines.Count() > 0)
            currentIndex = 0;
    }

    public static LinesReader FromText(string text)
    {
        return new LinesReader(text);
    }

    public static LinesReader FromStringList(string[] lines)
    {
        return new LinesReader(lines);
    }

    public string NextLine()
    {
        if (currentIndex == -1) return "-- NO TEXT RECEIVED --";
        if (currentIndex > lines.Count() - 1) return "-- READ BEYOND THE TEXT --";
        var result = lines[currentIndex];
        currentIndex++;
        if (trimLines)
            return result.Trim();
        return result;
    }

    public LinesReader TrimLines()
    {
        trimLines = true;
        return this;
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