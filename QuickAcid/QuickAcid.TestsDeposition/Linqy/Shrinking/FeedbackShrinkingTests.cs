using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy;

public static class Chapter { public const string Order = "1-50-50"; }

[Doc(Order = Chapter.Order, Caption = "Feedback Shrinking", Content =
@"A.k.a.: What if it fails but the run does not contain the minimal fail case ? 
")]
public class FeedbackShrinkingTests
{
    [Fact]
    public void What_is_a_single_runner()
    {
        Assert.IsType<QAcidRunner<int>>("an int".Shrinkable(MGen.Int()));
    }
}