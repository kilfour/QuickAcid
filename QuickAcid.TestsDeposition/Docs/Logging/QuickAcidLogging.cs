using QuickExplainIt.Text;
using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;
using QuickExplainIt;

namespace QuickAcid.TestsDeposition.Docs.Logging;

public static class Chapter { public const string Order = "1-99"; }

[Doc(Order = Chapter.Order, Caption = "QuickAcid Logging", Content =
@"Let's not call a spade a shovel: property-based testing (PBT) isn't the easiest thing in the world.
I usually frown upon verbosity and an overdose of logging, but here, it's not just tolerable, it's necessary.
Especially when the user starts to dig a little deeper and implements its own custom shrinkers for instance.  

There are two ways to get diagnostics out of QuickAcid's engine.
")]
public class QuickAcidLogging
{
    [Fact]
    [Doc(Order = $"{Chapter.Order}-1", Caption = "Verbose Mode", Content =
@"By adding a call to `.Verbose()` when building your test, you instruct the engine to include detailed diagnostic output in the report.
```csharp
var script =
    from spec in ""spec"".Spec(() => false)
    select Acid.Test;
new QState(script).Verbose().Testify(1);
```
This will produce a report that contains :
")]
    public void Verbose_enabling()
    {
        var script =
            from spec in "spec".Spec(() => true)
            select Acid.Test;
        new QState(script).Verbose().Testify(1);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-2", Content = " - Information about the first failed run.")]
    public void Verbose_contains_first_failed_run()
    {
        var script =
            from spec in "spec".Spec(() => false)
            select Acid.Test;
        var report = new QState(script).Verbose().ObserveOnce();
        Assert.NotNull(report);
        Assert.Equal("FIRST FAILED RUN", report.First<ReportTitleSectionEntry>().Title[0]);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-3", Content =
@" - Information about the run after execution shrinking.
")]
    public void Verbose_contains_after_execution_shrinking()
    {
        var script =
            from spec in "spec".Spec(() => false)
            select Acid.Test;
        var report = new QState(script).Verbose().ObserveOnce();
        Assert.NotNull(report);
        Assert.Equal("AFTER EXECUTION SHRINKING", report.Second<ReportTitleSectionEntry>().Title[0]);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-3.5", Content =
@" - Information about the run after action shrinking.
")]
    public void Verbose_contains_after_action_shrinking()
    {
        var script =
            from spec in "spec".Spec(() => false)
            select Acid.Test;
        var report = new QState(script).ShrinkingActions().Verbose().ObserveOnce();
        Assert.NotNull(report);
        Assert.Equal("AFTER ACTION SHRINKING", report.Third<ReportTitleSectionEntry>().Title[0]);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-4", Content =
@" - Information about the run after input shrinking.
")]
    public void Verbose_contains_after_input_shrinking()
    {
        var script =
            from spec in "spec".Spec(() => false)
            select Acid.Test;
        var report = new QState(script).ShrinkingActions().Verbose().ObserveOnce();
        Assert.NotNull(report);
        Assert.Equal("AFTER INPUT SHRINKING :", report.Fourth<ReportTitleSectionEntry>().Title[0]);
    }

    [Fact]
    [Doc(Order = $"{Chapter.Order}-5", Content =
@"Which for this example: 
```csharp
var script =
    from container in ""stashed"".Stashed(() => new Container(0))
    from input in ""input"".Input(MGen.Int(1, 6))
    from act in ""act"".Act(() => container.Value = input)
    from spec in ""spec"".Spec(() => container.Value != 5)
    select Acid.Test;
new QState(script).Verbose().Testify(50);
```
Outputs something similar to:
```
 ----------------------------------------
 -- FIRST FAILED RUN
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : act
   - Input : input = 2
 ---------------------------
 EXECUTE : act
   - Input : input = 2
 ---------------------------
 EXECUTE : act
   - Input : input = 3
 ---------------------------
 EXECUTE : act
   - Input : input = 2
 ---------------------------
 EXECUTE : act
   - Input : input = 5
 *******************
  Spec Failed : spec
 *******************

 ----------------------------------------
 -- AFTER EXECUTION SHRINKING
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : act
   - Input : input = 5
 *******************
  Spec Failed : spec
 *******************

 ----------------------------------------
 -- AFTER INPUT SHRINKING :
 -- Property 'spec' was falsified
 -- Original failing run: 5 execution(s)
 -- Shrunk to minimal case:  1 execution(s) (6 shrinks)
 ----------------------------------------
 RUN START :
 ---------------------------
 EXECUTE : act
   - Input : input = 5
 *******************
  Spec Failed : spec
 *******************
 ```
")]
    public void Verbose_full_output()
    {
        var script =
            from container in "stashed".Stashed(() => new Container<int>(0))
            from input in "input".Input(MGen.Constant(5))
            from act in "act".Act(() => container.Value = input)
            from spec in "spec".Spec(() => container.Value != 5)
            select Acid.Test;
        var report = new QState(script, 666).ShrinkingActions().Verbose().Observe(20);
        var reader = LinesReader.FromText(report.ToString());
        Assert.Equal("QuickAcid Report:", reader.NextLine());
        Assert.Equal(" ----------------------------------------", reader.NextLine());
        Assert.Equal(" -- FIRST FAILED RUN", reader.NextLine());
        Assert.Equal(" ----------------------------------------", reader.NextLine());
        Assert.Equal(" RUN START :", reader.NextLine());
        Assert.Equal(" ---------------------------", reader.NextLine());
        Assert.Equal(" EXECUTE : act", reader.NextLine());
        Assert.Equal("   - Input : input = 5", reader.NextLine());
        Assert.Equal(" *******************", reader.NextLine());
        Assert.Equal("  Spec Failed : spec", reader.NextLine());
        Assert.Equal(" *******************", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal(" ----------------------------------------", reader.NextLine());
        Assert.Equal(" -- AFTER EXECUTION SHRINKING", reader.NextLine());
        Assert.Equal(" ----------------------------------------", reader.NextLine());
        Assert.Equal(" RUN START :", reader.NextLine());
        Assert.Equal(" ---------------------------", reader.NextLine());
        Assert.Equal(" EXECUTE : act", reader.NextLine());
        Assert.Equal("   - Input : input = 5", reader.NextLine());
        Assert.Equal(" *******************", reader.NextLine());
        Assert.Equal("  Spec Failed : spec", reader.NextLine());
        Assert.Equal(" *******************", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal(" ----------------------------------------", reader.NextLine());
        Assert.Equal(" -- AFTER ACTION SHRINKING", reader.NextLine());
        Assert.Equal(" ----------------------------------------", reader.NextLine());
        Assert.Equal(" RUN START :", reader.NextLine());
        Assert.Equal(" ---------------------------", reader.NextLine());
        Assert.Equal(" EXECUTE : act", reader.NextLine());
        Assert.Equal("   - Input : input = 5", reader.NextLine());
        Assert.Equal(" *******************", reader.NextLine());
        Assert.Equal("  Spec Failed : spec", reader.NextLine());
        Assert.Equal(" *******************", reader.NextLine());
        Assert.Equal("", reader.NextLine());
        Assert.Equal(" ----------------------------------------", reader.NextLine());
        Assert.Equal(" -- AFTER INPUT SHRINKING :", reader.NextLine());
        Assert.Equal(" -- Property 'spec' was falsified", reader.NextLine());
        Assert.Equal(" -- Original failing run: 1 execution(s)", reader.NextLine());
        Assert.Equal(" -- Shrunk to minimal case:  1 execution(s) (2 shrinks)", reader.NextLine());
        Assert.Equal(" -- Seed: 666", reader.NextLine());
        Assert.Equal(" ----------------------------------------", reader.NextLine());
        Assert.Equal(" RUN START :", reader.NextLine());
        Assert.Equal(" ---------------------------", reader.NextLine());
        Assert.Equal(" EXECUTE : act", reader.NextLine());
        Assert.Equal("   - Input : input = 5", reader.NextLine());
        Assert.Equal(" *******************", reader.NextLine());
        Assert.Equal("  Spec Failed : spec", reader.NextLine());
        Assert.Equal(" *******************", reader.NextLine());

    }
}