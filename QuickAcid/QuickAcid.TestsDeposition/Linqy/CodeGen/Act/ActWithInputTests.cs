using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;
using QuickMGenerate;

namespace QuickAcid.Tests.CodeGen.Act
{
    public class ActWithInputTests
    {
        [Fact]
        public void CodeGen_act_default()
        {
            var run =
                from s in "input".Input(MGen.Constant(42))
                from a in "DoingStuff:input".Act(() => { })
                select Acid.Test;

            var reader = LinesReader.FromRun(run);
            reader.Skip(4);
            Assert.Equal("    DoingStuff(42);", reader.NextLine());

        }
    }
}