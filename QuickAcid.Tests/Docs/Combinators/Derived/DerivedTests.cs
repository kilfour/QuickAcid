using QuickAcid.TestsDeposition._Tools;
using QuickPulse.Explains;
using QuickFuzzr;
using QuickAcid.Tests._Tools.ThePress;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Derived;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-100"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Derived", Content =
@"**Derived(...)** introduces a value that is dynamically generated during each execution, 
but is **not** shrinkable or tracked in the final report.  
Use this when you need to **react to mutable test state**, 
for example, selecting an input based on a previously `Stashed(...)` value.  

This is a niche combinator, 
primarily intended for state-sensitive generation where traditional shrinking would be inappropriate or misleading.

")]
public class DerivedTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from container in ""container"".Stashed(() => new Container<List<int>>([]))
from input in ""input"".Derived(Fuzz.ChooseFromWithDefaultWhenEmpty(container.Value))
```
")]
    [Fact]
    public void Derived_usage()
    {
        var script =
            from container in "container".Stashed(() => new Container<List<int>>([]))
            from input in "input".Derived(Fuzz.ChooseFromWithDefaultWhenEmpty(container.Value))
            from act in "act".Act(() => container.Value.Add(42))
            from spec in "fail".Spec(() => input != 42)
            select Acid.Test;
        var article = TheJournalist.Exposes(() => QState.Run(script).WithOneRun().And(5.ExecutionsPerRun()));
        Assert.Equal(0, article.Total().Inputs());
    }

    [Fact]
    public void Derived_alternative_usage()
    {
        var script =
            from container in "container".Stashed(() => new Container<List<int>>(new List<int>()))
            from input in "input".Derived(Fuzz.ChooseFromWithDefaultWhenEmpty(container.Value))
            select Acid.Test;
        var article = TheJournalist.Unearths(QState.Run(script).WithOneRunAndOneExecution());
        Assert.False(article.VerdictReached());
    }

    [Fact(Skip = "**not** shrinkable")]
    public void Derived_is_not_involved_in_shrinking()
    {
    }

    [Fact(Skip = "Tested Above")]
    public void Derived_reporting_ignored()
    {
    }
}

