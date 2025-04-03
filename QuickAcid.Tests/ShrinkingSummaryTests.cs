using QuickMGenerate;
[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace QuickAcid.Tests;

public class ShrinkingSummaryTests
{
    [Fact]
    public void FalsifiableException_Should_Include_ShrinkingSummary_With_MinimalInputsOnly()
    {
        var test =
            from alwaysFail in "irrelevant input".ShrinkableInput(MGen.Int(0, 10))
            from alsoAlwaysFail in "shrinking target".ShrinkableInput(MGen.Int(0, 100))
            from check in "fail".Spec(() => alwaysFail > 3 && alsoAlwaysFail < 0)
            select Acid.Test;

        var ex = Record.Exception(() => test.Verify(10, 1));

        Assert.NotNull(ex);
        Assert.IsType<FalsifiableException>(ex);

        var msg = ex.ToString();

#if DEBUG
        QAcidDebug.Write(((FalsifiableException)ex).MemoryDump);
#endif

        // Confirm shrink summary is included
        Assert.Contains(" shrinks", msg);

        // Only include inputs that matter to the spec
        Assert.DoesNotContain("irrelevant input", msg); // trimmed during shrinking
        Assert.Contains("shrinking target", msg); // essential
    }

    [Fact]
    public void Shrinking_Should_Remove_Unused_Inputs()
    {
        var test =
            from ignored in "not used".ShrinkableInput(MGen.Int(1, 100))
            from trigger in "failing value".ShrinkableInput(MGen.Int(0, 5))
            from check in "must fail".Spec(() => trigger != 3)
            select Acid.Test;

        var ex = Assert.Throws<FalsifiableException>(() => test.Verify(20, 1));
        var msg = ex.ToString();

        // The trigger is kept
        Assert.Contains("failing value", msg);

        // The unrelated input is not
        Assert.DoesNotContain("not used", msg);
    }

    [Fact]
    public void Shrinking_Should_Remove_Unused_Inputs_Tricky()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "log.txt");
        var test =
            from ignored in "not used".ShrinkableInput(MGen.Int(1, 100))
            from trigger in "failing value".ShrinkableInput(MGen.Int(0, 5))
            from check in "must fail".Spec(() =>
            {
                if (trigger == 3)
                {
                    return false;
                }
                return true;
            })
            select Acid.Test;

        var ex = Assert.Throws<FalsifiableException>(() => test.Verify(20, 1));
        var msg = ex.ToString();


        // The trigger is kept
        Assert.Contains("failing value", msg);

        // The unrelated input is not
        Assert.DoesNotContain("not used", msg);
    }
}