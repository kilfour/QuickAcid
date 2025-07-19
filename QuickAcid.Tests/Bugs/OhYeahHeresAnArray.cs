using QuickFuzzr;

namespace QuickAcid.Tests.Bugs;

public class OhYeahHeresAnArray
{
    [Fact(Skip = "next on the list")]
    public void BoemBoem()
    {
        var counter = 0;
        var script =
            from input in "input".Input(Fuzz.Constant(new[] { 1, 2 }))
            from act in "act".Act(() => counter++)
            from spec in "spec".SpecIf(() => counter > 3, () => false)
            select Acid.Test;
        new QState(script).Testify(100);
    }
}