using QuickPulse.Explains;
using StringExtensionCombinators;

namespace QuickAcid.TestsDeposition.Docs.Combinators.Skip;

[DocFile]
[DocContent(
@"**Skip(...)** extension method for `QAcidScript<T>`. Useful for temporary disabling `Spec`'s.
")]
public class SkipTests
{
    [Fact]
    [DocContent(
@"**Usage example:**
```csharp
from __ in ""spec"".Spec(() => false).Skip()
```
")]
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

    [Fact]
    [DocContent(
@"**SkipIf(...)** is the same as `Skip(...)` but only skips conditionally.  

**Usage example:**
```csharp
from __ in ""spec"".Spec(() => false).SkipIf(() => true)
```
")]
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

