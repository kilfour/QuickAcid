using QuickMGenerate;

namespace QuickAcid.Tests;

public class ShrinkingSummaryTests
{
    [Fact]
    public void FalsifiableException_Should_Include_ShrinkingSummary_With_MinimalInputsOnly()
    {
        var test =
            from alwaysFail in "irrelevant input".ShrinkableInput(MGen.Int(0, 10))
            from check in "fail".Spec(() => false)
            select Acid.Test;

        var ex = Record.Exception(() => test.Verify(1, 1));

        Assert.NotNull(ex);
        Assert.IsType<FalsifiableException>(ex);

        var msg = ex.ToString();

        // Confirm shrink summary is included
        Assert.Contains(" shrinks", msg);
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
                QAcidDebug.WriteLine($"[spec evaluation] trigger = {trigger}");
                if (trigger == 3)
                    QAcidDebug.WriteLine(">> SPEC FAILED");
                return (trigger != 3);
            })
            select Acid.Test;

        var ex = Assert.Throws<FalsifiableException>(() => test.Verify(50, 1));
        var msg = ex.ToString();
        QAcidDebug.Write(ex.MemoryDump);

        // The trigger is kept
        Assert.Contains("failing value", msg);

        // The unrelated input is not
        Assert.DoesNotContain("not used", msg);


    }
}