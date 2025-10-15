using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Docs.Shrinking.Primitives.KnownPrimitives;

public class DateOnlyTests
{
    [Fact]
    public void Simple()
    {
        var script =
            from i1 in "i1".Input(Fuzz.Constant(new DateOnly(2025, 1, 1)))
            from i2 in "i2".Input(Fuzz.Constant(new DateOnly(2025, 1, 1)))
            from spec in "spec".Spec(() => i2 != new DateOnly(2025, 1, 1))
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
                .WithOneRunAndOneExecution());

        Assert.Equal(1, article.Total().Inputs());
        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.Equal("i2", inputEntry.Label);
        Assert.Equal("2025-01-01", inputEntry.Value);
    }
}