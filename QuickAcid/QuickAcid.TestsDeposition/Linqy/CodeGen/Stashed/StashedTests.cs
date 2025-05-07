using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickAcid.TestsDeposition._Tools;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen.Stashed;

public class StashedTests
{
    [Fact]
    public void CodeGen_stashed_default()
    {
        var run =
            from _ in "MyObject".Stashed(() => new object())
            select Acid.Test;
        var reader = Reader.FromRun(run);
        Assert.Equal("var myObject = new MyObject();", reader.NextLine());
    }

    [Fact]
    public void CodeGen_two_stashed_default()
    {
        var run =
            from _ in "MyObject".Stashed(() => new object())
            from __ in "MyOtherObject".Stashed(() => new object())
            select Acid.Test;
        var reader = Reader.FromRun(run);
        Assert.Equal("var myObject = new MyObject();", reader.NextLine());
        Assert.Equal("var myOtherObject = new MyOtherObject();", reader.NextLine());
    }
}