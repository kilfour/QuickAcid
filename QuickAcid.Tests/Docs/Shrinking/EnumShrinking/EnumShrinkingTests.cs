using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Docs.Shrinking.EnumShrinking;

public class EnumShrinkingTests
{
    [Fact]
    public void SystemEnum()
    {
        var script =
            from i1 in "i1".Input(Fuzz.Constant(DayOfWeek.Monday))
            from i2 in "i2".Input(Fuzz.Constant(DayOfWeek.Monday))
            from spec in "spec".Spec(() => i2 != DayOfWeek.Monday)
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
                .WithOneRunAndOneExecution());

        Assert.Equal(1, article.Total().Inputs());
        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.Equal("i2", inputEntry.Label);
        Assert.Equal("Monday", inputEntry.Value);
    }

    enum MyEnum { One, Two }

    [Fact]
    public void CustomEnum()
    {
        var script =
            from i1 in "i1".Input(Fuzz.Constant(MyEnum.One))
            from i2 in "i2".Input(Fuzz.Constant(MyEnum.One))
            from spec in "spec".Spec(() => i2 != MyEnum.One)
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script)
                .WithOneRunAndOneExecution());

        Assert.Equal(1, article.Total().Inputs());
        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.Equal("i2", inputEntry.Label);
        Assert.Equal("One", inputEntry.Value);
    }
}