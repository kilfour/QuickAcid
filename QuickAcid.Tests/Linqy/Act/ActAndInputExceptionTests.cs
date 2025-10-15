using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Linqy.Act;

public class ActAndInputExceptionTests
{
    [Fact]
    public void ExceptionThrownByAct()
    {
        var script =
            from input in "input".Input(Fuzz.Int(1, 1))
            from foo in "foo".Act(() => { if (input == 1) throw new Exception(); })
            from spec in "spec".Spec(() => true)
            select Acid.Test;

        var article = TheJournalist.Exposes(() => QState.Run(script)
            .WithOneRun()
            .AndOneExecutionPerRun());

        var inputEntry = article.Execution(1).Input(1).Read();
        Assert.NotNull(inputEntry);
        Assert.Equal("input", inputEntry.Label);

        var actEntry = article.Execution(1).Action(1).Read();
        Assert.NotNull(actEntry);
        Assert.Equal("foo", actEntry.Label);
    }
}