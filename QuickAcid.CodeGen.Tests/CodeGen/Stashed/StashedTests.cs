using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen.Stashed;

public class StashedTests
{
    [Fact]
    public void CodeGen_stashed()
    {
        var script =
            from _ in "MyObject".Stashed(() => new object())
            select Acid.Test;
        var reader = Reader.FromRun(script);
        Assert.Equal("var myObject = new MyObject();", reader.NextLine());
    }

    [Fact]
    public void CodeGen_stashed_ctor_arg()
    {
        var script =
            from _ in "MyObject".Stashed(() => new object())
            from __ in "MyOtherObject:myObject".Stashed(() => new object())
            select Acid.Test;
        var reader = Reader.FromRun(script);
        Assert.Equal("var myObject = new MyObject();", reader.NextLine());
        Assert.Equal("var myOtherObject = new MyOtherObject(myObject);", reader.NextLine());
    }

    [Fact]
    public void CodeGen_two_stashed()
    {
        var script =
            from _ in "MyObject".Stashed(() => new object())
            from __ in "MyOtherObject".Stashed(() => new object())
            select Acid.Test;
        var reader = Reader.FromRun(script);
        Assert.Equal("var myObject = new MyObject();", reader.NextLine());
        Assert.Equal("var myOtherObject = new MyOtherObject();", reader.NextLine());
    }
}