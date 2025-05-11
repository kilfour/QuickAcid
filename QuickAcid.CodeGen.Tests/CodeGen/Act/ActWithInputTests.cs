using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate;

namespace QuickAcid.TestsDeposition.Linqy.CodeGen.Act
{
    public class ActWithInputTests
    {
        [Fact]
        public void CodeGen_act_one_arg()
        {
            var script =
                from s in "input".Input(MGen.Constant(42))
                from a in "DoingStuff:input".Act(() => { })
                select Acid.Test;
            var reader = Reader.FromRun(script);
            Assert.Equal("DoingStuff(42);", reader.NextLine());
        }
    }
}