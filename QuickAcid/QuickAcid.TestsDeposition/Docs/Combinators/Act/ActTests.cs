using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.Reporting;
using QuickAcid.TestsDeposition._Tools;
using QuickAcid.TestsDeposition._Tools.Models;
using QuickMGenerate;


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
        var run =
            from account in "Account".Stashed(() => new Account())
            from act in "act".Act(() => account.Withdraw(500))
            from spec in "fail".Spec(() => false)
            select Acid.Test;
        var report = new QState(run).Observe(1);
        var entry = report.FirstOrDefault<ReportExecutionEntry>();
        Assert.NotNull(entry);
        Assert.Equal("act", entry.Key);
    }
}

