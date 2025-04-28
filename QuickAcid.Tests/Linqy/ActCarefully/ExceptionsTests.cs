using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate;

namespace QuickAcid.Tests.Linqy.ActCarefully;


public class ExceptionsTests
{
    [Fact]
    public void HowToCatchA()
    {
        var run =
            from result in "act".ActCarefully(() => { throw new Exception(); })
            from throws in "throws".Spec(result.Throws<Exception>)
            select Acid.Test;

        var report = run.ReportIfFailed();
        Assert.Null(report);
    }
    public class ExceptionOne : Exception { }
    public class ExceptionTwo : Exception { }

    [Fact]
    public void HowToCatchMultiple()
    {
        var run =
            from flag in "flag".Shrinkable(MGen.Bool())
            from result1 in "act1".ActCarefully(() => { if (flag) throw new Exception(); })
            from result2 in "act2".ActCarefully(() => { if (!flag) throw new Exception(); })
            from throws in "throws".Spec(() => result1.Throws<Exception>() || result2.Throws<Exception>())
            select Acid.Test;

        var report = run.ReportIfFailed();
        Assert.Null(report);
    }
}





