using QuickAcid.Tests._Tools.ThePress;
using QuickFuzzr;

namespace QuickAcid.Tests.Bugs;

public class SpecNotEvaluated
{
    [Fact]
    public void Using_A_Value_That_Is_Not_Supposed_To_Be_Null_But_Shrinking_Tries_It_Anyway()
    {
        var script =
            from input in "input".Input(Fuzz.String())
            from act in "act".Act(() => { if (input == null) throw new Exception("Boom"); })
            from spec in "spec".Spec(() => false)
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script).WithOneRunAndOneExecution());

        Assert.Equal(0, article.Total().Inputs());
    }

    [Fact]
    public void Try_A_Number()
    {
        var script =
            from input in "input".Input(Fuzz.Constant(42))
            from act in "act".Act(() => { return 5 * input; })
            from spec in "spec".Spec(() => input != 42)
            select Acid.Test;

        var article = TheJournalist.Exposes(() =>
            QState.Run(script).WithOneRunAndOneExecution());

        Assert.Equal(1, article.Total().Inputs());
    }


}