using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Tests.CodeGen.Act
{
    public class ActTests
    {
        [Fact]
        public void CodeGen_act_default()
        {
            var run = from _ in "The_Key_From_Test".Act(() => { }) select Acid.Test;

            var reader = LinesReader.FromRun(run);
            Assert.Equal("[Fact]", reader.NextLine());
            Assert.Equal("public void Throws()", reader.NextLine());
            Assert.Equal("{", reader.NextLine());
            Assert.Equal("", reader.NextLine());
            Assert.Equal("    The_Key_From_Test();", reader.NextLine());
            Assert.Equal("    Assert.Throws(--------- NOT YET ---------);", reader.NextLine());
            Assert.Equal("}", reader.NextLine());
            Assert.Equal("", reader.NextLine());
        }

        [Fact]
        public void CodeGen_two_acts_default()
        {
            var run =
                from _1 in "The_Key_From_Test1".Act(() => { })
                from _2 in "The_Key_From_Test2".Act(() => { })
                select Acid.Test;

            var reader = LinesReader.FromRun(run);
            reader.Skip(4);
            Assert.Equal("    The_Key_From_Test1();", reader.NextLine());
            Assert.Equal("    The_Key_From_Test2();", reader.NextLine());
        }
    }
}