using QuickAcid.Bolts;
using StringExtensionCombinators;

namespace QuickAcid.Tests.Linqy.Spec;

public class SpecExceptionTests
{
    [Fact]
    public void ExceptionThrownBySpecIsNotAQuickAcidException()
    {

        var script = from spec in "spec".Spec(Throw) select Acid.Test;
        var ex = Assert.Throws<Exception>(() => QState.Run(script).WithOneRunAndOneExecution());
        Assert.IsNotType<FalsifiableException>(ex);
        Assert.Contains("QuickAcid.Tests.Linqy.Spec.SpecExceptionTests.Throw()", ex.StackTrace);
    }

    private bool Throw()
    {
        throw new Exception();
    }
}