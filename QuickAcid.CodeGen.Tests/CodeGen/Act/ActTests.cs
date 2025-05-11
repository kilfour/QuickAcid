using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen.Act
{
    public class ActTests
    {
        [Fact]
        public void CodeGen_act()
        {
            var script = from _ in "The_Key_From_Test".Act(() => { }) select Acid.Test;
            var reader = Reader.FromRun(script);
            Assert.Equal("The_Key_From_Test();", reader.NextLine());
        }

        [Fact]
        public void CodeGen_two_acts()
        {
            var script =
                from _1 in "The_Key_From_Test1".Act(() => { })
                from _2 in "The_Key_From_Test2".Act(() => { })
                select Acid.Test;
            var reader = Reader.FromRun(script);
            Assert.Equal("The_Key_From_Test1();", reader.NextLine());
            Assert.Equal("The_Key_From_Test2();", reader.NextLine());
        }
    }
}