using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Tests.CodeGen.Stashed
{
    public class StashedTests
    {
        [Fact]
        public void CodeGen_stashed_default()
        {
            var run =
                from _ in "MyObject".Stashed(() => new object())
                select Acid.Test;
            var reader = LinesReader.FromRun(run);
            Assert.Equal("[Fact]", reader.NextLine());
            Assert.Equal("public void Throws()", reader.NextLine());
            Assert.Equal("{", reader.NextLine());
            Assert.Equal("    var myObject = new MyObject();", reader.NextLine());
            Assert.Equal("    Assert.Throws(--------- NOT YET ---------);", reader.NextLine());
            Assert.Equal("}", reader.NextLine());
            Assert.Equal("", reader.NextLine());
            Assert.True(reader.EndOfCode());
        }

        [Fact]
        public void CodeGen_two_stashed_default()
        {
            var run =
                from _ in "MyObject".Stashed(() => new object())
                from __ in "MyOtherObject".Stashed(() => new object())
                select Acid.Test;
            var reader = LinesReader.FromRun(run);
            reader.Skip(3);
            Assert.Equal("    var myObject = new MyObject();", reader.NextLine());
            Assert.Equal("    var myOtherObject = new MyOtherObject();", reader.NextLine());
        }
    }
}