namespace QuickAcid.Proceedings;

public class TestMethodInfoDeposition
{
    public string MethodName { get; }
    public string SourceFile { get; }
    public int LineNumber { get; }

    public TestMethodInfoDeposition(string methodName, string sourceFile, int lineNumber)
    {
        MethodName = methodName;
        SourceFile = sourceFile;
        LineNumber = lineNumber;
    }
};