using QuickPulse.Explains;
using QuickFuzzr;
using QuickAcid.Tests._Tools.ThePress;
using StringExtensionCombinators;

namespace QuickAcid.TestsDeposition.Docs.Combinators.Input;


[DocFile]
[DocContent(
@"**Input(...)** introduces a fuzzed value that will be regenerated for every execution and shrunk when a failure occurs.  
It represents the core mechanism for exploring input space in property-based tests.  
Use it when you want a variable value that's subject to shrinking and included in the final report upon failure.

This is the most common kind of test input — think of it as the default for fuzzable values.")
]
public class InputTests
{
    [Fact]
    [DocContent(
@"**Note:** If an input is not involved in a failing execution, it will not appear in the report.
")]
    public void UnusedInputIsNotReported()
    {
        var script =
            from input in "input".Input(Fuzzr.Int())
            from foo in "spec".Spec(() => false)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());
        Assert.Equal(0, article.Total().Inputs());
    }

    [Fact]
    [DocContent(
@"**Usage example:**
```csharp
from input in ""input"".Input(() => Fuzzr.Int())
```
")]
    public void Input_usage()
    {
        var script =
            from input in "input".Input(Fuzzr.Int())
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
    }

    [Fact]
    public void Null_shows_up_in_report_as_null()
    {
        var script =
            from input in "input".Input(Fuzzr.Constant<string?>(null))
            from foo in "spec".Spec(() => !string.IsNullOrEmpty(input))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var deposition = article.Execution(1).Input(1).Read();
        Assert.Equal("input", deposition.Label);
        Assert.Equal("null", deposition.Value);
    }

    [Fact]
    public void Empty_String_shows_up_in_report_as_empty()
    {
        var script =
            from input in "input".Input(Fuzzr.Constant(""))
            from foo in "spec".Spec(() => !string.IsNullOrEmpty(input))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var deposition = article.Execution(1).Input(1).Read();
        Assert.Equal("input", deposition.Label);
        Assert.Equal("\"\"", deposition.Value);
    }

    [Fact]
    public void String_shows_up_in_report_with_quotes()
    {
        var script =
            from input in "input".Input(Fuzzr.Constant(" "))
            from foo in "spec".Spec(() => !string.IsNullOrWhiteSpace(input))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var deposition = article.Execution(1).Input(1).Read();
        Assert.Equal("input", deposition.Label);
        Assert.Equal("\" \"", deposition.Value);
    }
}

