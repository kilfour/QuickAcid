using QuickAcid.Reporting;
using QuickExplainIt;
using QuickFuzzr;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Input;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-50"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Input", Content =
@"**Input(...)** introduces a fuzzed value that will be regenerated for every execution and shrunk when a failure occurs.  
It represents the core mechanism for exploring input space in property-based tests.  
Use it when you want a variable value that's subject to shrinking and included in the final report upon failure.

This is the most common kind of test input — think of it as the default for fuzzable values.")
]
public class InputTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Note:** If an input is not involved in a failing execution, it will not appear in the report.
")]
    [Fact]
    public void UnusedInputIsNotReported()
    {
        var script =
            from input in "input".Input(MGen.Int())
            from foo in "spec".Spec(() => false)
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.Null(entry);
    }

    [Doc(Order = $"{Chapter.Order}-2", Content =
@"**Usage example:**
```csharp
from input in ""input"".Input(() => MGen.Int())
```
")]
    [Fact]
    public void Input_usage()
    {
        var script =
            from input in "input".Input(MGen.Int())
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
    }

    [Fact]
    public void Null_shows_up_in_report_as_null()
    {
        var script =
            from input in "input".Input(MGen.Constant<string?>(null))
            from foo in "spec".Spec(() => !string.IsNullOrEmpty(input))
            select Acid.Test;

        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("<null>", entry.Value);
    }

    [Fact]
    public void Empty_String_shows_up_in_report_as_empty()
    {
        var script =
            from input in "input".Input(MGen.Constant(""))
            from foo in "spec".Spec(() => !string.IsNullOrEmpty(input))
            select Acid.Test;

        var report = new QState(script).ObserveOnce(); ;
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("\"\"", entry.Value);
    }

    [Fact]
    public void String_shows_up_in_report_with_quotes()
    {
        var script =
            from input in "input".Input(MGen.Constant(" "))
            from foo in "spec".Spec(() => !string.IsNullOrWhiteSpace(input))
            select Acid.Test;

        var report = new QState(script).ObserveOnce(); ;
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("\" \"", entry.Value);
    }
}

