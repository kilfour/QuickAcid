using QuickMGenerate;

namespace QuickAcid.Tests;

public class ShrinkingSummaryTests
{
    [Fact]
    public void FalsifiableException_Should_Include_ShrinkingSummary()
    {
        var test =
            from x in "bad value".ShrinkableInput(MGen.Int(0, 10))
            from check in "fail check".Spec(() => x > 5 && x < 6) // Impossible
            select Acid.Test;

        var ex = Record.Exception(() => test.Verify(10, 1));

        Assert.NotNull(ex);
        Assert.IsType<FalsifiableException>(ex);

        var msg = ex.ToString();

        Assert.Contains("Shrinks attempted", msg); // summary should show
        Assert.Contains("bad value", msg);         // shrunk input should be present
    }
}