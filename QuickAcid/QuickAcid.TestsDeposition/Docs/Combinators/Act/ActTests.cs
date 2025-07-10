using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickAcid.TestsDeposition._Tools.Models;


namespace QuickAcid.TestsDeposition.Docs.Combinators.Act;

public static class Chapter { public const string Order = CombinatorChapter.Order + "-60"; }

[Doc(Order = $"{Chapter.Order}", Caption = "Act", Content =
@"**Act(...)** is your go to when you want to mutate your system under test.
####TODO: elaborate
")]
public class ActTests
{
    [Doc(Order = $"{Chapter.Order}-1", Content =
@"**Usage example:**
```csharp
from act in ""act"".Act(() => account.Withdraw(500))
```

")]
    [Fact]
    public void Act_usage()
    {
        var script =
            from account in "Account".Stashed(() => new Account())
            from act in "act".Act(() => account.Withdraw(500))
            from spec in "fail".Spec(() => account.Balance >= 0)
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("act", entry.Key);
    }

    [Doc(Order = $"{Chapter.Order}-5", Content =
@"An overload of this combinator exists which can return a value, and therefor pass it down the LINQ chain.
```csharp
from act in ""act"".Act(() => account.GetBalance())
```
")]
    [Fact]
    public void Act_can_return_value()
    {
        var script =
            from act in "act".Act(() => 42)
            from spec in "fail".Spec(() => act != 42)
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        Assert.NotNull(report);
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("act", entry.Key);
    }
    [Doc(Order = $"{Chapter.Order}-10", Content =
@"**Mutiple acts in one execution => can't shrink ! not the way to model things**
```csharp
from act1 in ""act once"".Act(() => account.Withdraw(500))
from act2 in ""and act again"".Act(() => account.Withdraw(200))
```
")]
    [Fact]
    public void Act_multiple_one_execution()
    {
        var script =
            from account in "Account".Stashed(() => new Account())
            from act1 in "act once".Act(() => account.Withdraw(20))
            from act2 in "and act again".Act(() => account.Withdraw(20))
            from spec in "fail".Spec(() => account.Balance >= -30)
            select Acid.Test;
        var report = new QState(script).ObserveOnce();
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("act once, and act again", entry.Key);
    }
}

