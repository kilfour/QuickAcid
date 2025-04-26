using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Tests.CodeGen.Act
{
    public class AlwaysReportedTests
    {
        [Fact]
        public void CodeGen_always_reported_default()
        {
            var run =
                from _ in "MyObject".AlwaysReported(() => new object())
                select Acid.Test;
            var reader = CodeReader.FromRun(run);
            Assert.Equal("[Fact]", reader.NextLine());
            Assert.Equal("public void Throws()", reader.NextLine());
            Assert.Equal("{", reader.NextLine());
            Assert.Equal("    var myObject = new MyObject();", reader.NextLine());
            Assert.Equal("", reader.NextLine());
            Assert.Equal("    Assert.Throws(--------- NOT YET ---------);", reader.NextLine());
            Assert.Equal("}", reader.NextLine());
            Assert.Equal("", reader.NextLine());
            Assert.True(reader.EndOfCode());
        }

        [Fact]
        public void CodeGen_two_always_reported_default()
        {
            var run =
                from _ in "MyObject".AlwaysReported(() => new object())
                from __ in "MyOtherObject".AlwaysReported(() => new object())
                select Acid.Test;
            var reader = CodeReader.FromRun(run);
            reader.Skip(3);
            Assert.Equal("    var myObject = new MyObject();", reader.NextLine());
            Assert.Equal("    var myOtherObject = new MyOtherObject();", reader.NextLine());
        }
    }
}