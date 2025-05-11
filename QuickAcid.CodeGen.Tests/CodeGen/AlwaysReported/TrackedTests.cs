using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen.AlwaysReported;

public class TrackedTests
{
    [Fact]
    public void CodeGen_always_reported()
    {
        var script =
            from _ in "MyObject".Tracked(() => new object())
            select Acid.Test;
        var reader = Reader.FromRun(script);
        Assert.Equal("var myObject = new MyObject();", reader.NextLine());
    }

    [Fact]
    public void CodeGen_two_always_reported()
    {
        var script =
            from _ in "MyObject".Tracked(() => new object())
            from __ in "MyOtherObject".Tracked(() => new object())
            select Acid.Test;
        var reader = Reader.FromRun(script);
        Assert.Equal("var myObject = new MyObject();", reader.NextLine());
        Assert.Equal("var myOtherObject = new MyOtherObject();", reader.NextLine());
    }
}