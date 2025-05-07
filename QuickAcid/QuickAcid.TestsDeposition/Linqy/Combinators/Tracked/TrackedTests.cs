using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickAcid.TestsDeposition._Tools.Models;
using QuickMGenerate;
using QuickPulse;
using QuickPulse.Diagnostics.Sinks.FileWriters;


namespace QuickAcid.TestsDeposition.Linqy.Combinators.Tracked;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-20"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Tracked", Content =
@"**Tracked(...)** behaves exactly like `Stashed(...)`: it defines a named value that is created once at the start of the test run and shared across all executions.
The key difference is that `Tracked(...) `also ensures this value is **always included in the final report**, providing visibility into contextual or setup state even when it wasn't directly responsible for the failure.
")]
public class TrackedTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from account in ""account"".Tracked(() => new Account(), a => a.Balance.ToString())
```
The second argument is a formatter function for rendering the value into the test report.
")]
    [Fact]
    public void Tracked_usage_and_stringify()
    {
        var run =
            from account in "account".Tracked(() => new Account(), a => a.Balance.ToString())
            select Acid.Test;
        var report = new QState(run).AlwaysReport().Observe(1);
        var entry = report.FirstOrDefault<ReportTrackedEntry>();
        Assert.NotNull(entry);
        Assert.Equal("   => account (tracked) : 0", entry.ToString());
    }

    [Fact]
    public void Tracked_in_report_after_shrinking()
    {
        var run =
           from container in "container".Tracked(() => new Container<int>(21), a => a.Value.ToString())
           from input in "input".Input(MGen.Constant(42))
           from _do in "do".Act(() => { container.Value = input; })
           from _ in "spec".Spec(() => container.Value != 42)
           select Acid.Test;

        var report = new QState(run).ObserveOnce();
        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportTrackedEntry>();
        Assert.NotNull(entry);
        Assert.Equal("21", entry.Value);
    }

    [Fact]
    public void Tracked_only_initialized_at_start_of_run()
    {
        var executionCount = 0;
        var run =
            from token in "container".Tracked(() =>
            {
                if (executionCount != 0)
                    ComputerSays.No();
                return new Container<bool>(true);
            })
            from act in "act".Act(() => { executionCount++; })
            select Acid.Test;
        new QState(run).Observe(3);
    }

    //[Fact]
    // public void Tracked_should_be_consistent_across_executions()
    // {
    //     var executionCount = 0;
    //     var specHolds = true;
    //     var alwaysReportedChanged = false;
    //     var run =
    //         from token in "token".Tracked(() =>
    //         {
    //             if (specHolds && executionCount == 1)
    //                 alwaysReportedChanged = true;
    //             executionCount = 0;
    //             specHolds = true;
    //             return 42;
    //         })
    //         from observe in "observe".Act(() => { executionCount++; })
    //         from delayedFail in "fail".Spec(() =>
    //             {
    //                 specHolds = executionCount <= 2;
    //                 return specHolds;
    //             }
    //         )
    //         select Acid.Test;

    //     new QState(run).AlwaysReport().Observe(3);
    //     Assert.False(alwaysReportedChanged);
    // }
    // [Fact]
    // public void Tracked_should_exist_in_per_action_report()
    // {
    //     var report =
    //         SystemSpecs
    //             .Define()
    //             .Tracked(Keys.Container, () => new Container() { ItsOnlyAModel = 1 })
    //             .Do("throw", _ => throw new Exception())
    //             .DumpItInAcid()
    //             .AndCheckForGold(1, 1);
    //     Assert.NotNull(report);
    //     var entry = report.FirstOrDefault<ReportTrackedInputEntry>();
    //     Assert.NotNull(entry);
    //     Assert.Equal("container", entry.Key);
    //     Assert.Equal("   => container (tracked) : 1", entry.ToString());
    // }

    // [Fact]
    // public void TrackedInput_can_use_context_when_registering()
    // {
    //     var container = new Container();
    //     var report =
    //         SystemSpecs.Define()
    //             .Tracked(Keys.TheAnswer, () => 42)
    //             .Tracked(Keys.Universe, ctx => { container.ItsOnlyAModel = ctx.Get(Keys.TheAnswer); return container; })
    //             .DumpItInAcid()
    //             .AndCheckForGold(1, 1);
    //     Assert.Null(report);
    //     Assert.Equal(42, container.ItsOnlyAModel);
    // }

    // [Fact]
    // public void TrackedInput_can_be_stringified()
    // {
    //     var container = new Container();
    //     var report =
    //         SystemSpecs.Define()
    //             .Tracked(Keys.TheAnswer, () => 42, a => "it is " + a)
    //             .Do("throw", _ => throw new Exception())
    //             .DumpItInAcid()
    //             .AndCheckForGold(1, 1);
    //     Assert.NotNull(report);
    //     var entry = report.FirstOrDefault<ReportTrackedInputEntry>();
    //     Assert.NotNull(entry);
    //     Assert.Equal("theAnswer", entry.Key);
    //     Assert.Equal("   => theAnswer (tracked) : it is 42", entry.ToString());
    // }



    [Doc(Order = $"{Chapter.Order}-2-1", Caption = "TrackedValue", Content =
@"
Similar to `StashedValue(...)`, but again, this one shows up in the report.

**Example:**
```csharp
from flag in ""flag"".TrackedValue(true)
```
")]
    [Fact]
    public void StashedValue_usage()
    {
        var run =
            from flag in "flag".Tracked(true)
            select Acid.Test;
        new QState(run).Testify(1);
    }
}

