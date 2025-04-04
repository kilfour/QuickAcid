using QuickMGenerate;

namespace QuickAcid.Examples.CodeGen;

public class Spike
{
    [Fact]
    public void InitialTry()
    {
        var run =
            from input in "input".ShrinkableInput(MGen.Int(0, 10))
            from act in "act".Act(() => { })
            from spec in "spec".Spec(() => input != 3)
            select Acid.Test;
    }
}