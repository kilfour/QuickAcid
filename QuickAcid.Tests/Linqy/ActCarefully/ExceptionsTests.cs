using QuickFuzzr;

namespace QuickAcid.Tests.Linqy.ActCarefully;


public class ExceptionsTests
{
    [Fact]
    public void How_to_catch_one()
    {
        var script =
            from result in "act".ActCarefully(() => { throw new Exception(); })
            from throws in "throws".Spec(result.ThrewAs<Exception>)
            select Acid.Test;
        Assert.False(QState.Run(script).WithOneRunAndOneExecution().HasVerdict());
    }

    [Fact]
    public void How_to_catch_multiple()
    {
        var script =
            from flag in "flag".Input(Fuzz.Bool())
            from result1 in "act1".ActCarefully(() => { if (flag) throw new Exception(); })
            from result2 in "act2".ActCarefully(() => { if (!flag) throw new Exception(); })
            from throws in "throws".Spec(() => result1.ThrewAs<Exception>() || result2.ThrewAs<Exception>())
            select Acid.Test;
        Assert.False(QState.Run(script).WithOneRunAndOneExecution().HasVerdict());
    }

    [Fact]
    public void How_to_catch_one_out_of_two()
    {
        var script =
            from flag in "flag".Input(Fuzz.Bool())
            from result1 in "act1".ActCarefully(() => { })
            from result2 in "act2".ActCarefully(() => { if (flag) throw new Exception(); })
            from throws in "throws".Spec(() => !flag || result2.ThrewAs<Exception>())
            select Acid.Test;
        Assert.False(QState.Run(script).WithOneRunAndOneExecution().HasVerdict());
    }

    // Stack multiple DelayedResults	            ActCarefully many things, mix and match thrown/values
    // Pass DelayedResult into another Act	        Chain based on success/failure of prior acts
    // Shrink based on DelayedResults	            Shrinker path depends on whether exception occurred
}





