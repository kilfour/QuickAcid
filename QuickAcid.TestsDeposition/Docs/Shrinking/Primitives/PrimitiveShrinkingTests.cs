using QuickAcid.Reporting;
using QuickExplainIt;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Docs.Shrinking.Primitives;


public static class Chapter { public const string Order = "1-50-5"; }

[Doc(Order = Chapter.Order, Caption = "Primitives Shrinking", Content =
@"`PrimitiveShrinkStrategy` is a shrinker used in QuickAcid to simplify failing test inputs for known primitive types.
It operates using a predefined list of alternative values per type and attempts to
replace the original with a simpler version that still causes the test to fail.
")]
public class PrimitiveShrinkingTests
{
    [Doc(Order = Chapter.Order + "-1", Caption = "How It Works", Content =
@"1. **Recognize Known Type**  
   Checks if the given value belongs to a supported primitive type (like `int`, `bool`, `string`, etc.).
2. **Initial Check**  
   First ensures that the original value *actually* causes a failure. Otherwise, shrinking is skipped.
3. **Candidate Evaluation**
   - Iterates over alternative values (excluding the current one).
   - Tries all candidates. If one causes the test to pass, the current value is minimal.
4. **Trace Result**  
   After evaluation, it emits a trace indicating whether the value was kept, or marked irrelevant.
")]
    [Fact]
    public void OneRelevantInt()
    {
        var script =
            from input1 in "input1".Input(MGen.Int(5, 7))
            from input2 in "input2".Input(MGen.Int(5, 7))
            from foo in "act".Act(() =>
            {
                if (input1 == 6) throw new Exception();
            })
            select Acid.Test;

        var report = new QState(script).Observe(50);

        Assert.Single(report.OfType<ReportInputEntry>());

        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input1", inputEntry.Key);
        Assert.Equal("6", inputEntry.Value);

        var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Key);
        Assert.NotNull(report.Exception);
    }

    [Fact]
    public void TwoRelevantInts()
    {
        var script =
            from input1 in "input1".Input(MGen.Int(5, 7))
            from input2 in "input2".Input(MGen.Int(5, 7))
            from foo in "act".Act(() =>
            {
                if (input1 == 6 && input2 == 6) throw new Exception();
            })
            select Acid.Test;

        var report = new QState(script).Observe(50);

        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input1", inputEntry.Key);
        Assert.Equal("6", inputEntry.Value);

        inputEntry = report.SecondOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input2", inputEntry.Key);
        Assert.Equal("6", inputEntry.Value);

        var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Key);
        Assert.NotNull(report.Exception);
    }

    [Fact]
    public void TwoRelevantIntsTricky()
    {
        var script =
            from input1 in "input1".Input(MGen.Int(2, 4))
            from input2 in "input2".Input(MGen.Int(3, 5))
            from foo in "act".Act(() =>
            {
                if (input1 == 3 && input2 > 3) throw new Exception();
            })
            select Acid.Test;

        var report = new QState(script).Observe(100);

        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input1", inputEntry.Key);
        Assert.Equal("3", inputEntry.Value);

        inputEntry = report.SecondOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input2", inputEntry.Key);
        Assert.NotNull(inputEntry.Value);
        bool success = int.TryParse(inputEntry.Value.ToString(), out int input2FromReport);
        Assert.True(success);
        Assert.True(input2FromReport > 3);

        var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Key);
        Assert.NotNull(report.Exception);
    }

    [Doc(Order = Chapter.Order + "-2", Caption = "Supported Types", Content =
@"These are matched via `Type.IsAssignableFrom(...)` to allow some flexibility:

- **Boolean**: `true`, `false`
- **Numeric Types**: `int`, `long`, `short`, `byte`, `float`, `double`, `decimal`, including unsigned variants
- **Characters**: `\0`, `'a'`, `'Z'`, space, newline, `\uFFFF`
- **Strings**: `null`, empty, short strings, long strings
- **Time Types**: `DateTime`, `DateTimeOffset`, `TimeSpan`
- **Miscellaneous**: `Guid`, `Uri`
")]
    [Fact]
    public void OneRelevantChar()
    {
        var script =
            from input1 in "input1".Input(MGen.ChooseFrom(['X', 'Y']))
            from input2 in "input2".Input(MGen.Char())
            from foo in "act".Act(() =>
            {
                if (input1 == 'X') throw new Exception("Boom");
            })
            select Acid.Test;

        var report = new QState(script).Observe(50);

        var inputEntry = report.FirstOrDefault<ReportInputEntry>();
        Assert.NotNull(inputEntry);
        Assert.Equal("input1", inputEntry.Key);
        Assert.Equal("X", inputEntry.Value); // Could use .ToString(CultureInfo.InvariantCulture) if needed

        var actEntry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(actEntry);
        Assert.Equal("act", actEntry.Key);
        Assert.NotNull(report.Exception);
    }
}