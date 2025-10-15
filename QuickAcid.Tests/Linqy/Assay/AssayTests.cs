using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Linqy.Assay;

public class AssayTests
{
    [Fact]
    public void Assay_something_did_not_happen()
    {
        var script =
            from observer in "observer".Tracked(() => new HashSet<int>())
            from roll in "roll".Act(() => Fuzz.Int(1, 3).Generate())
            from _a1 in "record".Act(() => observer.Add(roll))
            from as1 in "gens 3".Assay(() => observer.Contains(3))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(20.ExecutionsPerRun()));

        Assert.Equal("gens 3", article.AssayerDisagrees());
    }

    [Fact]
    public void Assay_multiple_did_not_happen()
    {
        var script =
            from observer in "observer".Tracked(() => new HashSet<int>())
            from roll in "roll".Act(() => Fuzz.Int(1, 3).Generate())
            from _a1 in "record".Act(() => observer.Add(roll))
            from as1 in "combined".Assay(("gens 3", () => observer.Contains(3)), ("gens 4", () => observer.Contains(4)))
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .And(20.ExecutionsPerRun()));

        Assert.Equal("gens 3, gens 4", article.AssayerDisagrees());
    }
}