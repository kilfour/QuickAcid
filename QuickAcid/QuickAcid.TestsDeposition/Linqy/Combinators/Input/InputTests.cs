using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickAcid.TestsDeposition._Tools.Models;
using QuickMGenerate;


namespace QuickAcid.TestsDeposition.Linqy.Combinators.Input;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-50"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Input", Content =
@"**Input(...)** ...
")]
public class InputTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from input in ""input"".Input(() => MGen.Int())
```
")]
    [Fact]
    public void Input_usage()
    {
        var run =
            from input in "input".Input(MGen.Int())
            select Acid.Test;
        new QState(run).Testify(1);
    }

    [Fact]
    public void UnusedInputIsNotReported()
    {
        var run =
            from input in "input".Input(MGen.Int())
            from foo in "spec".Spec(() => false)
            select Acid.Test;
        var report = new QState(run).ObserveOnce();
        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.Null(entry);
    }

    [Fact]
    public void Null_shows_up_in_report_as_null()
    {
        var run =
            from input in "input".Input(MGen.Constant<string?>(null))
            from foo in "spec".Spec(() => !string.IsNullOrEmpty(input))
            select Acid.Test;

        var report = new QState(run).ObserveOnce();
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("<null>", entry.Value);
    }

    [Fact]
    public void Empty_String_shows_up_in_report_as_empty()
    {
        var run =
            from input in "input".Input(MGen.Constant(""))
            from foo in "spec".Spec(() => !string.IsNullOrEmpty(input))
            select Acid.Test;

        var report = new QState(run).ObserveOnce(); ;
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("\"\"", entry.Value);
    }

    [Fact]
    public void String_shows_up_in_report_with_quotes()
    {
        var run =
            from input in "input".Input(MGen.Constant(" "))
            from foo in "spec".Spec(() => !string.IsNullOrWhiteSpace(input))
            select Acid.Test;

        var report = new QState(run).ObserveOnce(); ;
        Assert.NotNull(report);

        var entry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(entry);
        Assert.Equal("input", entry.Key);
        Assert.Equal("\" \"", entry.Value);
    }
}

