using QuickAcid.Bolts;
using QuickAcid.Bolts.Nuts;

namespace QuickAcid.Tests.CodeGen.Act
{
    public class SpecTests
    {
        [Fact]
        public void CodeGen_spec_default()
        {
            var run = from _ in "TheSpec".Spec(() => false) select Acid.Test;

            var reader = CodeReader.FromFailingRun(run);
            Assert.Equal("[Fact]", reader.NextLine());
            Assert.Equal("public void TheSpec()", reader.NextLine());
            Assert.Equal("{", reader.NextLine());
            Assert.Equal("", reader.NextLine());
            Assert.Equal("    Assert.True(TheSpec);", reader.NextLine());
            Assert.Equal("}", reader.NextLine());
            Assert.Equal("", reader.NextLine());
        }

        [Fact]
        public void CodeGen_spec_default_function_name()
        {
            var run = from _ in "TheSpec".Spec(() => false) select Acid.Test;

            var reader = CodeReader.FromFailingRun(run);
            reader.Skip();
            Assert.Equal("public void TheSpec()", reader.NextLine());
        }

        [Fact]
        public void CodeGen_spec_default_function_name_colon_seperator()
        {
            var run = from _ in "TheSpec: ignore me".Spec(() => false) select Acid.Test;

            var reader = CodeReader.FromFailingRun(run);
            reader.Skip();
            Assert.Equal("public void TheSpec()", reader.NextLine());
        }
    }
}