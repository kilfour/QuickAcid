using QuickPulse.Explains;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Skip;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-3000"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Skip", Content =
@"**Skip(...)** extension method for `QAcidScript<T>`. Useful for temporary disabling `Spec`'s.
")]
public class SkipTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from __ in ""spec"".Spec(() => false).Skip()
```
")]
    [Fact]
    public void Skip_usage()
    {
        var counter = 0;
        var script =
            from _ in "act1".Act(() => { counter++; })
            from __ in "spec".Spec(() => false).Skip()
            from ___ in "act2".Act(() => { counter++; })
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
        Assert.Equal(2, counter);
    }

    [Doc(Order = $"{Chapter.Order}-2", Content =
@"**SkipIf(...)** is the same as `Skip(...)` but only skips conditionally.  

**Usage example:**
```csharp
from __ in ""spec"".Spec(() => false).SkipIf(() => true)
```
")]
    [Fact]
    public void SkipIf_usage()
    {
        var counter = 0;
        var script =
            from _ in "act1".Act(() => { counter++; })
            from __ in "spec".Spec(() => false).SkipIf(() => true)
            from ___ in "act2".Act(() => { counter++; })
            select Acid.Test;
        QState.Run(script).WithOneRunAndOneExecution();
        Assert.Equal(2, counter);
    }
}

